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
    float tearringangle;
    public Material tearPartMaterial;

    Vector3 startPoint, endPoint, previousDraggingPosition;
    bool dragging, initialVertexCreated, firstChunk;
    string outputText = "Did not start";
    float groundLimit = 0.001f;

    GameObject tearMeshPart, vertexSpinePart, previousSpine;
    Mesh mesh;
    MeshFilter meshFilter;
    Vector3[] vertices;
    int[] triangles;
    Vector3 newVertex0, newVertex1, lastVertex0, lastVertex1;
    Vector3 travallingDirection;
    Color[] originalMeshColors;

    Vector3[] originalVertices;
    int[] originalTriangles;
    int[] triangleMap;
    Vector2[] uvs;
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
                initialVertexCreated = false;
                previousDraggingPosition = raycastHit.point;

                tearMeshPart = new GameObject("Tear Mesh Part");
                tearMeshPart.transform.position = previousDraggingPosition;

                previousSpine = null;
                vertexSpinePart = new GameObject("Vertex Spine Part");
                vertexSpinePart.transform.position = previousDraggingPosition;

                meshFilter = tearMeshPart.AddComponent<MeshFilter>();
                tearMeshPart.AddComponent<MeshRenderer>().material = tearPartMaterial;
                mesh = new Mesh();
                meshFilter.mesh = mesh;

                //originalMeshColors = new Color[meshDoctor.GetMesh().vertices.Length];
                //originalMeshColors = meshDoctor.GetMesh().colors;
                //if (originalMeshColors.Length == 0)
                //{
                //    originalMeshColors = new Color[meshDoctor.GetMesh().vertices.Length];
                //    for (int i = 0; i < originalMeshColors.Length; i++)
                //    {
                //        originalMeshColors[i] = Color.white;
                //    }
                //}
                //meshDoctor.GetMesh().colors = originalMeshColors;
                //vertices = new Vector3[] { };
                //triangle = new int[] { };
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

                    Vector3 side1 = (previousDraggingPosition - raycastHit.point) * Vector3.Distance(previousDraggingPosition, raycastHit.point);
                    Vector3 side2 = raycastHit.normal;
                    Vector3 tangentDir = Vector3.Cross(side1, side2).normalized * cuttingSize;
                    if (!initialVertexCreated)
                    {
                        

                        lastVertex0 = transform.TransformPoint(previousDraggingPosition + tangentDir);
                        lastVertex1 = transform.TransformPoint(previousDraggingPosition - tangentDir);
                        CreateVertexSpine(lastVertex0, lastVertex1);

                        //Debug.DrawLine(previousDraggingPosition, raycastHit.point, Color.blue, 10);
                        //Debug.DrawRay(previousDraggingPosition, tangentDir, Color.red, 1000);
                        //Debug.DrawRay(previousDraggingPosition, -tangentDir, Color.red, 1000);

                        initialVertexCreated = true;
                    }

                    if (!firstChunk)
                    {
                        if( Mathf.Abs(Vector3.Dot(travallingDirection, previousDraggingPosition - raycastHit.point)) >= tearingMaxAngle / 90)
                        {
                            Debug.LogError("Angle: " + Mathf.Abs(Vector3.Dot(travallingDirection, previousDraggingPosition - raycastHit.point)));
                            dragging = false;
                        }
                    }

                    newVertex0 = transform.TransformPoint(raycastHit.point + tangentDir);
                    newVertex1 = transform.TransformPoint(raycastHit.point - tangentDir);                    
                    CreateVertexSpine(newVertex0, newVertex1);

                    TearChunk tearChunk = previousSpine.GetComponent<TearChunk>();
                    vertices = new Vector3[] {
                        tearMeshPart.transform.InverseTransformPoint(tearChunk.vertex0.transform.position),
                        tearMeshPart.transform.InverseTransformPoint(tearChunk.vertex1.transform.position)
                    };
                    triangles = new int[] { };                    
                    do
                    {
                        Array.Resize(ref vertices, vertices.Length + 2);
                        vertices[vertices.Length - 2] = tearMeshPart.transform.InverseTransformPoint(tearChunk.previousTearChunk.vertex0.transform.position);
                        vertices[vertices.Length - 1] = tearMeshPart.transform.InverseTransformPoint(tearChunk.previousTearChunk.vertex1.transform.position);
                        vertices[vertices.Length - 2].y = vertices[vertices.Length - 2].y < groundLimit ? groundLimit : vertices[vertices.Length - 2].y;
                        vertices[vertices.Length - 1].y = vertices[vertices.Length - 1].y < groundLimit ? groundLimit : vertices[vertices.Length - 1].y;


                        Array.Resize(ref triangles, triangles.Length + 6);
                        triangles[triangles.Length - 6] = vertices.Length - 2;
                        triangles[triangles.Length - 5] = vertices.Length - 1;
                        triangles[triangles.Length - 4] = vertices.Length - 3;
                        triangles[triangles.Length - 3] = vertices.Length - 2;
                        triangles[triangles.Length - 2] = vertices.Length - 3;
                        triangles[triangles.Length - 1] = vertices.Length - 4;

                        tearChunk = tearChunk.previousTearChunk;
                    } while (tearChunk.previousTearChunk != null);

                    uvs = new Vector2[vertices.Length];
                    for (int i = 0; i < uvs.Length; i++)
                    {
                        uvs[i] = new Vector2( vertices[i].x, vertices[i].z);
                    }


                    //Debug.DrawRay(raycastHit.point, tangentDir, Color.red, 1000);
                    //Debug.DrawRay(raycastHit.point, -tangentDir, Color.red, 1000);


                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    mesh.uv = uvs;
                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                    mesh.RecalculateTangents();

                    travallingDirection = raycastHit.point - previousDraggingPosition;
                    previousDraggingPosition = raycastHit.point;
                    lastVertex0 = newVertex0;
                    lastVertex1 = newVertex1;
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
    void CreateVertexSpine(Vector3 point0, Vector3 point1)
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

        GameObject centerVertex = new GameObject("VertexCenter");
        GameObject vertex0 = new GameObject("point0");
        GameObject vertex1 = new GameObject("point1");

        centerVertex.transform.localPosition = (point0 + point1) / 2;
        vertex0.transform.localPosition = point0;
        vertex1.transform.localPosition = point1;
        //tearMeshPart.transform.InverseTransformPoint(point0);
        //tearMeshPart.transform.InverseTransformPoint(point1);


        vertex0.transform.parent = centerVertex.transform;
        vertex1.transform.parent = centerVertex.transform;

        TearChunk latestTearChunk = centerVertex.AddComponent<TearChunk>();
        latestTearChunk.vertex0 = vertex0;
        latestTearChunk.vertex1 = vertex1;

        if (previousSpine == null)
        {
            centerVertex.transform.parent = vertexSpinePart.transform;
        }
        else
        {
            previousSpine.transform.parent = centerVertex.transform;
            previousSpine.transform.Rotate(point0 - point1, foldingAngle);

            TearChunk previousTearChunk = previousSpine.GetComponent<TearChunk>();
            latestTearChunk.previousTearChunk = previousTearChunk;

            //ClearNewChankVertices(point0, point1, previousTearChunk.originalVertexPosition0, previousTearChunk.originalVertexPosition1);
            int[] tris = originalTriangles;

            Mesh _mesh = meshDoctor.GetMesh();
            GameObject newChunkMesh = new GameObject("NewChankMesh");
            newChunkMesh.layer = ConstantManager.BOUNDARY_LAYER;
            MeshFilter newFilter = newChunkMesh.AddComponent<MeshFilter>();
            Mesh newMesh = new Mesh();
            newFilter.mesh = newMesh;
            newMesh.vertices = new Vector3[] {
                    newChunkMesh.transform.InverseTransformPoint(point0),
                    newChunkMesh.transform.InverseTransformPoint(point1),
                    newChunkMesh.transform.InverseTransformPoint(previousTearChunk.originalVertexPosition0),
                    newChunkMesh.transform.InverseTransformPoint(previousTearChunk.originalVertexPosition1)
                };
            newMesh.triangles = new int[] {
                    1,0,2,1,2,3
                };
            newChunkMesh.AddComponent<MeshCollider>();
            List<Vector3> allHitPoints = new List<Vector3>(); 
            for (int i = 0; i < tris.Length; i += 3)
            {
                Vector3 v0 = meshDoctor.transform.TransformPoint(originalVertices[tris[i]]);
                Vector3 v1 = meshDoctor.transform.TransformPoint(originalVertices[tris[i + 1]]);
                Vector3 v2 = meshDoctor.transform.TransformPoint(originalVertices[tris[i + 2]]);
                Vector3 center = (v0 + v1 + v2) / 3;

                RaycastHit hit;
                if (Physics.Raycast(new Ray(center.ModifyThisVector(0, 1, 0), Vector3.down), out hit, 10, 1 << ConstantManager.BOUNDARY_LAYER))
                {
                    triangleMap[tris[i]] = 1;
                    originalTriangles[i] = 0;
                    originalTriangles[i + 1] = 0;
                    originalTriangles[i + 2] = 0;

                    allHitPoints.Add(center);
                }
            }
            Destroy(newChunkMesh);
            meshDoctor.UpdateMesh(originalVertices, originalTriangles, meshDoctor.originalUVs);

            float latestDistance0 = 100000;
            float latestDistance1 = 100000;
            float previousDistance0 = 100000;
            float previousDistance1 = 100000;
            for (int i = 0; i < allHitPoints.Count; i++)
            {
                if (Vector3.Distance(point0, allHitPoints[i]) < latestDistance0)
                {
                    latestDistance0 = Vector3.Distance(point0, allHitPoints[i]);
                    vertex0.transform.position = allHitPoints[i];
                }
                if (Vector3.Distance(point1, allHitPoints[i]) < latestDistance1)
                {
                    latestDistance1 = Vector3.Distance(point1, allHitPoints[i]);
                    vertex1.transform.position = allHitPoints[i];
                }
                if (Vector3.Distance(previousTearChunk.originalVertexPosition0, allHitPoints[i]) < previousDistance0)
                {
                    previousDistance0 = Vector3.Distance(previousTearChunk.originalVertexPosition0, allHitPoints[i]);
                    previousTearChunk.vertex0.transform.position = allHitPoints[i];
                }
                if (Vector3.Distance(previousTearChunk.originalVertexPosition1, allHitPoints[i]) < previousDistance1)
                {
                    previousDistance1 = Vector3.Distance(previousTearChunk.originalVertexPosition1, allHitPoints[i]);
                    previousTearChunk.vertex1.transform.position = allHitPoints[i];
                }
            }
            //point0 = closePoint;
            point0 = vertex0.transform.position;
            point1 = vertex1.transform.position;
            //GameObject go = new GameObject("Update Point");
            //go.transform.position = closePoint;
        }

        latestTearChunk.originalVertexPosition0 = point0;
        latestTearChunk.originalVertexPosition1 = point1;
        previousSpine = centerVertex;
    }

    void ClearNewChankVertices(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3)
    {
        Mesh _mesh = meshDoctor.GetMesh();
        GameObject newChankMesh = new GameObject("NewChankMesh");
        newChankMesh.layer = ConstantManager.BOUNDARY_LAYER;
        MeshFilter newFilter = newChankMesh.AddComponent<MeshFilter>();
        Mesh newMesh = new Mesh();
        newFilter.mesh = newMesh;
        newMesh.vertices = new Vector3[] {
                    newChankMesh.transform.InverseTransformPoint(point0),
                    newChankMesh.transform.InverseTransformPoint(point1),
                    newChankMesh.transform.InverseTransformPoint(point2),
                    newChankMesh.transform.InverseTransformPoint(point3)
                };
        newMesh.triangles = new int[] {
                    1,0,2,1,2,3
                };
        newChankMesh.AddComponent<MeshCollider>();
        for (int i = 0; i < _mesh.vertices.Length; i++)
        {
            Vector3 worldPoint = meshDoctor.transform.TransformPoint(_mesh.vertices[i]);
            RaycastHit hit;
            if (Physics.Raycast(new Ray(worldPoint.ModifyThisVector(0, 1, 0), Vector3.down), out hit, 10, 1 << ConstantManager.BOUNDARY_LAYER))
            {
                originalMeshColors[i] = new Color(255, 255, 255, 0);
                Debug.LogError(hit.collider.name);
            }
        }

        int[] tris = originalTriangles;

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = originalVertices[tris[i]];
            Vector3 v1 = originalVertices[tris[i + 1]];
            Vector3 v2 = originalVertices[tris[i + 2]];
            Vector3 center = meshDoctor.transform.TransformPoint(v0 + v1 + v2) / 3;
        }

        Destroy(newChankMesh);
        meshDoctor.UpdateMesh(originalVertices, tris, meshDoctor.originalUVs);
        _mesh.colors = originalMeshColors;
    }

    #endregion ALL SELF DECLEAR FUNCTIONS

}
