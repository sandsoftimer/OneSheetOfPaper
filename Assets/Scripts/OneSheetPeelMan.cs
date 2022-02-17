using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Com.AlphaPotato.Utility;
using System;

public class OneSheetPeelMan : APBehaviour
{
    public MeshDoctor meshDoctor;
    public TextMeshProUGUI text;

    [Header("Level Rules")]
    public float cuttingSize;
    public float draggingthreshold;
    public float foldingAngle;
    public float tearingMaxAngle;
    public Material tearPartMaterial;

    Vector3 startPoint, endPoint, previousDraggingPosition;
    bool dragging, initialVertexPairCreated, firstChunk;
    string outputText = "Did not start";
    float groundLimit = 0.001f;

    PairVertex previousPairVertex;
    GameObject tearMeshPart, vertexSpinePart;
    Mesh mesh;
    MeshFilter meshFilter;
    Vector3[] vertices;
    int[] triangles;
    Vector3 newVertex0, newVertex1;
    Vector3 travallingDirection;
    Color[] originalMeshColors;

    Vector3[] originalVertices;
    int[] originalTriangles;
    int[] triangleMap;
    Vector2[] originalUVs;
    List<Vector2> uvs;
    List<TriangleData> allTriangleData;
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        originalVertices = meshDoctor.originalVertices;
        originalTriangles = meshDoctor.originalTriangles;
        originalUVs = meshDoctor.originalUVs;

        triangleMap = new int[originalTriangles.Length / 3];
        for (int i = 0; i < triangleMap.Length; i++)
        {
            triangleMap[i] = 0;
        }
        meshDoctor.UpdateMesh(meshDoctor.originalVertices, meshDoctor.originalTriangles, meshDoctor.originalUVs);
    }

    void Update()
    {
        //if (gameState.Equals(GameState.GAME_INITIALIZED) && Input.GetMouseButtonDown(0))
        //{
        //    gameManager.ChangeGameState(GameState.GAME_PLAY_STARTED);
        //    gameState = GameState.GAME_PLAY_STARTED;
        //}

        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

        if (dragging)
        {
            //outputText = APTools.mathManager.GetDirectionToPosition(previousDraggingPosition).ToString();
        }
        //if (Input.GetMouseButtonUp(0))
        //{
        //    endPoint = Input.mousePosition;
        //    dragging = false;
        //    outputText = APTools.mathManager.GetDirectionToPosition(startPoint).ToString();
        //    text.text = outputText;
        //}
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit = new RaycastHit();
            raycastHit.GetRaycastFromScreenTouch(1 << gameObject.layer);

            if (raycastHit.collider != null)
            {
                dragging = true;
                firstChunk = true;
                //allNewChunkValidVertices = new List<TriangleData>();
                uvs = new List<Vector2>();
                initialVertexPairCreated = false;
                previousDraggingPosition = raycastHit.point;

                tearMeshPart = new GameObject("Tear Mesh Part");
                tearMeshPart.transform.position = previousDraggingPosition;

                previousPairVertex = null;
                vertexSpinePart = new GameObject("Vertex Spine Part");
                vertexSpinePart.transform.position = previousDraggingPosition;

                meshFilter = tearMeshPart.AddComponent<MeshFilter>();
                tearMeshPart.AddComponent<MeshRenderer>().material = tearPartMaterial;
                mesh = new Mesh();
                meshFilter.mesh = mesh;
            }            
        }

        if (Input.GetMouseButton(0) && dragging)
        {
            RaycastHit raycastHit = new RaycastHit();
            raycastHit.GetRaycastFromScreenTouch(1 << gameObject.layer);

            if (raycastHit.collider != null)
            {
                if (Vector3.Distance(raycastHit.point, previousDraggingPosition) > draggingthreshold)
                {
                    //if (triangleMap[raycastHit.triangleIndex] != 0)
                    //{
                    //    GameObject go = new GameObject("Cancle Point");
                    //    Vector3 p0 = meshDoctor.transform.TransformPoint(originalVertices[originalTriangles[raycastHit.triangleIndex * 3]]);
                    //    Vector3 p1 = meshDoctor.transform.TransformPoint(originalVertices[originalTriangles[raycastHit.triangleIndex * 3 + 1]]);
                    //    Vector3 p2 = meshDoctor.transform.TransformPoint(originalVertices[originalTriangles[raycastHit.triangleIndex * 3 + 2]]);
                    //    Vector3 center = (p0 + p1 + p2) / 3;
                    //    go.transform.position = center;

                    //    Debug.LogError("Dragging Cancle");
                    //    dragging = false;
                    //    return;
                    //}

                    Vector3 side1 = previousDraggingPosition - raycastHit.point;
                    Vector3 side2 = raycastHit.normal;
                    Vector3 tangentDir = Vector3.Cross(side1, side2).normalized * cuttingSize;
                    if (!initialVertexPairCreated)
                    {
                        CreateVertexBone(
                            transform.TransformPoint(previousDraggingPosition + tangentDir),
                            transform.TransformPoint(previousDraggingPosition - tangentDir));

                        //Debug.DrawLine(previousDraggingPosition, raycastHit.point, Color.blue, 10);
                        //Debug.DrawRay(previousDraggingPosition, tangentDir, Color.red, 1000);
                        //Debug.DrawRay(previousDraggingPosition, -tangentDir, Color.red, 1000);

                        initialVertexPairCreated = true;
                    }

                    //if (!firstChunk)
                    //{
                    //    if( Mathf.Abs(Vector3.Dot(travallingDirection, previousDraggingPosition - raycastHit.point)) >= tearingMaxAngle / 90)
                    //    {
                    //        Debug.LogError("Angle: " + Mathf.Abs(Vector3.Dot(travallingDirection, previousDraggingPosition - raycastHit.point)));
                    //        dragging = false;
                    //    }
                    //}

                    newVertex0 = transform.TransformPoint(raycastHit.point + tangentDir);
                    newVertex1 = transform.TransformPoint(raycastHit.point - tangentDir);                    
                    CreateVertexBone(newVertex0, newVertex1);

                    PairVertex currentPairVertex = previousPairVertex.GetComponent<PairVertex>();
                    vertices = new Vector3[] {
                        tearMeshPart.transform.InverseTransformPoint(currentPairVertex.vertexObject0.transform.position),
                        tearMeshPart.transform.InverseTransformPoint(currentPairVertex.vertexObject1.transform.position)
                    };
                    triangles = new int[] { };                    
                    do
                    {
                        Array.Resize(ref vertices, vertices.Length + 2);
                        vertices[vertices.Length - 2] = tearMeshPart.transform.InverseTransformPoint(currentPairVertex.previousPairVertex.vertexObject0.transform.position);
                        vertices[vertices.Length - 1] = tearMeshPart.transform.InverseTransformPoint(currentPairVertex.previousPairVertex.vertexObject1.transform.position);
                        vertices[vertices.Length - 2].y = vertices[vertices.Length - 2].y < groundLimit ? groundLimit : vertices[vertices.Length - 2].y;
                        vertices[vertices.Length - 1].y = vertices[vertices.Length - 1].y < groundLimit ? groundLimit : vertices[vertices.Length - 1].y;


                        Array.Resize(ref triangles, triangles.Length + 6);
                        triangles[triangles.Length - 6] = vertices.Length - 2;
                        triangles[triangles.Length - 5] = vertices.Length - 1;
                        triangles[triangles.Length - 4] = vertices.Length - 3;
                        triangles[triangles.Length - 3] = vertices.Length - 2;
                        triangles[triangles.Length - 2] = vertices.Length - 3;
                        triangles[triangles.Length - 1] = vertices.Length - 4;

                        currentPairVertex = currentPairVertex.previousPairVertex;
                    } while (currentPairVertex.previousPairVertex != null);

                    //uvs = new Vector2[allNewChunkValidVertices.Count];
                    //Debug.LogError(vertices.Length + " -- " + uvs.Count);
                    //for (int i = 0; i < uvs.Count; i++)
                    //{
                    //    Debug.LogError("uv index " + i + ":" + uvs[i]);
                    //}
                    //Debug.LogError("==============");


                    //Debug.DrawRay(raycastHit.point, tangentDir, Color.red, 1000);
                    //Debug.DrawRay(raycastHit.point, -tangentDir, Color.red, 1000);

                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    mesh.uv = uvs.ToArray();
                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                    mesh.RecalculateTangents();

                    travallingDirection = raycastHit.point - previousDraggingPosition;
                    previousDraggingPosition = raycastHit.point;
                    //dragging = false;
                    firstChunk = false;
                }
            }            
        }
    }

    void FixedUpdate()
    {
        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

    }

    void LateUpdate()
    {
        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

        //if (dragging)
        //    previousDraggingPosition = APTools.mathManager.GetWorldTouchPosition(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        //dragging = true;
        //startPoint = APTools.mathManager.GetWorldTouchPosition(Input.mousePosition);
        //previousDraggingPosition = APTools.mathManager.GetWorldTouchPosition(Input.mousePosition);
        //outputText = "Dragging";
    }

    #endregion ALL UNITY FUNCTIONS
    //=================================   
    #region ALL OVERRIDING FUNCTIONS


    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS

    int count = 0;
    //List<TriangleData> allNewChunkValidVertices;
    void CreateVertexBone(Vector3 point0, Vector3 point1)
    {
        //if(previousSpine != null)
        //{
        //    Debug.LogError(++count + " : " + Vector3.Distance(point0, previousSpine.GetComponent<TearChunk>().originalVertexPosition0));
        //    float distance = Vector3.Distance(point0, previousSpine.GetComponent<TearChunk>().originalVertexPosition0);
        //    if(distance > draggingthreshold)
        //    {
        //        Vector3 newp0 = point0 * Time.deltaTime;
        //        Vector3 newp1 = point1 * Time.deltaTime;
        //        CreateVertexSpine(newp0, newp1);
        //    }
        //}

        GameObject pairVertexHolder = new GameObject("pairVertexHolder");
        GameObject vertexObject0 = new GameObject("vertexObj0");
        GameObject vertexObject1 = new GameObject("vertexObj1");

        pairVertexHolder.transform.position = (point0 + point1) / 2;
        //vertexObj0.transform.position = point0;
        //vertexObj1.transform.position = point1;

        vertexObject0.transform.parent = pairVertexHolder.transform;
        vertexObject1.transform.parent = pairVertexHolder.transform;

        PairVertex currentPairVertex = pairVertexHolder.AddComponent<PairVertex>();
        currentPairVertex.SetVertex(vertexObject0, vertexObject1, point0, point1);

        if (initialVertexPairCreated)
        {
            #region Precise new_chank(4 VERRICES)
            currentPairVertex.previousPairVertex = previousPairVertex;

            GameObject newChunkMesh = new GameObject("NewChankMesh");
            newChunkMesh.layer = ConstantManager.BOUNDARY_LAYER;
            MeshFilter newFilter = newChunkMesh.AddComponent<MeshFilter>();
            Mesh newMesh = new Mesh();
            newFilter.mesh = newMesh;
            newMesh.vertices = new Vector3[] {
                    currentPairVertex.vertexPosition0,
                    currentPairVertex.vertexPosition1,
                    currentPairVertex.previousPairVertex.vertexPosition0,
                    currentPairVertex.previousPairVertex.vertexPosition1
                };
            newMesh.triangles = new int[] {
                    1,0,2,1,2,3
                };
            newChunkMesh.AddComponent<MeshCollider>();
            //List<Vector3> allValidVertices = new List<Vector3>();
            allTriangleData = new List<TriangleData>();
            for (int i = 0; i < originalTriangles.Length; i += 3)
            {
                Vector3 v0 = transform.TransformPoint(originalVertices[originalTriangles[i]]);
                Vector3 v1 = transform.TransformPoint(originalVertices[originalTriangles[i + 1]]);
                Vector3 v2 = transform.TransformPoint(originalVertices[originalTriangles[i + 2]]);
                TriangleData td = new TriangleData()
                {
                    vertex = (v0 + v1 + v2) / 3,
                    vertexIndex = i
                };

                RaycastHit hit;
                if (Physics.Raycast(new Ray(td.vertex.ModifyThisVector(0, 1, 0), Vector3.down), out hit, 2, 1 << ConstantManager.BOUNDARY_LAYER))
                {
                    if (Physics.Raycast(new Ray(v0.ModifyThisVector(0, 1, 0), Vector3.down), out hit, 2, 1 << ConstantManager.BOUNDARY_LAYER))
                    {
                        //allValidVertices.Add(v0);
                        TriangleData triangleData = new TriangleData()
                        {
                            vertex = v0,
                            vertexIndex = originalTriangles[i]
                        };
                        allTriangleData.Add(triangleData);
                    }
                    if (Physics.Raycast(new Ray(v1.ModifyThisVector(0, 1, 0), Vector3.down), out hit, 2, 1 << ConstantManager.BOUNDARY_LAYER))
                    {
                        //allValidVertices.Add(v1);
                        TriangleData triangleData = new TriangleData()
                        {
                            vertex = v1,
                            vertexIndex = originalTriangles[i + 1]
                        };
                        allTriangleData.Add(triangleData);
                    }
                    if (Physics.Raycast(new Ray(v2.ModifyThisVector(0, 1, 0), Vector3.down), out hit, 2, 1 << ConstantManager.BOUNDARY_LAYER))
                    {
                        //allValidVertices.Add(v2);
                        TriangleData triangleData = new TriangleData()
                        {
                            vertex = v2,
                            vertexIndex = originalTriangles[i + 2]
                        };
                        allTriangleData.Add(triangleData);
                    }
                    triangleMap[originalTriangles[i]] = 1;
                    originalTriangles[i] = 0;
                    originalTriangles[i + 1] = 0;
                    originalTriangles[i + 2] = 0;
                }
            }
            Destroy(newChunkMesh);
            meshDoctor.UpdateMesh(originalVertices, originalTriangles, meshDoctor.originalUVs);

            int id0 = -1, id1 = -1, id2 = -1, id3 = -1;
            float latestDistance0 = 100000;
            float latestDistance1 = 100000;
            float previousDistance0 = 100000;
            float previousDistance1 = 100000;
            for (int i = 0; i < allTriangleData.Count; i++)
            {
                if (Vector3.Distance(point0, allTriangleData[i].vertex) < latestDistance0)
                {
                    latestDistance0 = Vector3.Distance(point0, allTriangleData[i].vertex);
                    id0 = i;
                }
                if (Vector3.Distance(point1, allTriangleData[i].vertex) < latestDistance1)
                {
                    latestDistance1 = Vector3.Distance(point1, allTriangleData[i].vertex);
                    id1 = i;
                }

                if (firstChunk)
                {
                    if (Vector3.Distance(currentPairVertex.previousPairVertex.vertexPosition0, allTriangleData[i].vertex) < previousDistance0)
                    {
                        previousDistance0 = Vector3.Distance(currentPairVertex.previousPairVertex.vertexPosition0, allTriangleData[i].vertex);
                        id2 = i;
                    }
                    if (Vector3.Distance(currentPairVertex.previousPairVertex.vertexPosition1, allTriangleData[i].vertex) < previousDistance1)
                    {
                        previousDistance1 = Vector3.Distance(currentPairVertex.previousPairVertex.vertexPosition1, allTriangleData[i].vertex);
                        id3 = i;
                    }
                }
            }

            if (id0 != -1)
            {
                point0 = allTriangleData[id0].vertex;
                uvs.Add(originalUVs[allTriangleData[id0].vertexIndex]);
                Debug.LogError("id0: " + id0 + " ///Vertex Index: " + allTriangleData[id0].vertexIndex);
            }
            if (id1 != -1)
            {
                point1 = allTriangleData[id1].vertex;
                uvs.Add(originalUVs[allTriangleData[id1].vertexIndex]);
            }

            currentPairVertex.SetVertex(
                vertexObject0,
                vertexObject1,
                point0,
                point1);

            if (firstChunk)
            {
                if (id2 != -1)
                {
                    currentPairVertex.previousPairVertex.vertexPosition0 = allTriangleData[id2].vertex;
                    uvs.Add(originalUVs[allTriangleData[id2].vertexIndex]);
                }
                if (id3 != -1)
                {
                    currentPairVertex.previousPairVertex.vertexPosition1 = allTriangleData[id3].vertex;
                    uvs.Add(originalUVs[allTriangleData[id3].vertexIndex]);
                }

                currentPairVertex.previousPairVertex.SetVertex(
                    currentPairVertex.previousPairVertex.vertexObject0,
                    currentPairVertex.previousPairVertex.vertexObject1,
                    currentPairVertex.previousPairVertex.vertexPosition0,
                    currentPairVertex.previousPairVertex.vertexPosition1);
            }
            #endregion Precise new_chank(4 VERRICES)

            previousPairVertex.transform.parent = pairVertexHolder.transform;
            pairVertexHolder.transform.Rotate(point0 - point1, foldingAngle);

        }

        previousPairVertex = currentPairVertex;
    }

    #endregion ALL SELF DECLEAR FUNCTIONS

}