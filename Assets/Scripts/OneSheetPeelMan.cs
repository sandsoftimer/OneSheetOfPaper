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
    float tearringangle;
    public Material tearPartMaterial;

    Vector3 startPoint, endPoint, previousDraggingPosition;
    bool dragging, initialVertexCreated;
    string outputText = "Did not start";
    float groundLimit = 0.001f;

    GameObject tearMeshPart, vertexSpinePart, previousSpine;
    Mesh mesh;
    MeshFilter meshFilter;
    Vector3[] vertices;
    int[] triangle;
    Vector3 newVertex0, newVertex1, lastVertex0, lastVertex1;
    Color[] originalMeshColors;

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

                Vector3[] vertpos = meshDoctor.GetMesh().vertices;
                originalMeshColors = new Color[meshDoctor.GetMesh().vertices.Length];
                originalMeshColors = meshDoctor.GetMesh().colors;
                if (originalMeshColors.Length == 0)
                {
                    originalMeshColors = new Color[meshDoctor.GetMesh().vertices.Length];
                    for (int i = 0; i < originalMeshColors.Length; i++)
                    {
                        originalMeshColors[i] = Color.white;
                    }
                }
                meshDoctor.GetMesh().colors = originalMeshColors;
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

                    newVertex0 = transform.TransformPoint(raycastHit.point + tangentDir);
                    newVertex1 = transform.TransformPoint(raycastHit.point - tangentDir);                    
                    CreateVertexSpine(newVertex0, newVertex1);

                    TearChank tearchank = previousSpine.GetComponent<TearChank>();
                    vertices = new Vector3[] {
                        tearMeshPart.transform.InverseTransformPoint(tearchank.vertex0.transform.position),
                        tearMeshPart.transform.InverseTransformPoint(tearchank.vertex1.transform.position)
                    };
                    triangle = new int[] { };                    
                    do
                    {
                        Array.Resize(ref vertices, vertices.Length + 2);
                        vertices[vertices.Length - 2] = tearMeshPart.transform.InverseTransformPoint(tearchank.previousTearChank.vertex0.transform.position);
                        vertices[vertices.Length - 1] = tearMeshPart.transform.InverseTransformPoint(tearchank.previousTearChank.vertex1.transform.position);
                        vertices[vertices.Length - 2].y = vertices[vertices.Length - 2].y < groundLimit ? groundLimit : vertices[vertices.Length - 2].y;
                        vertices[vertices.Length - 1].y = vertices[vertices.Length - 1].y < groundLimit ? groundLimit : vertices[vertices.Length - 1].y;


                        Array.Resize(ref triangle, triangle.Length + 6);
                        triangle[triangle.Length - 6] = vertices.Length - 2;
                        triangle[triangle.Length - 5] = vertices.Length - 1;
                        triangle[triangle.Length - 4] = vertices.Length - 3;
                        triangle[triangle.Length - 3] = vertices.Length - 2;
                        triangle[triangle.Length - 2] = vertices.Length - 3;
                        triangle[triangle.Length - 1] = vertices.Length - 4;

                        tearchank = tearchank.previousTearChank;
                    } while (tearchank.previousTearChank != null);


                    //Debug.DrawRay(raycastHit.point, tangentDir, Color.red, 1000);
                    //Debug.DrawRay(raycastHit.point, -tangentDir, Color.red, 1000);


                    mesh.vertices = vertices;
                    mesh.triangles = triangle;
                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                    mesh.RecalculateTangents();

                    previousDraggingPosition = raycastHit.point;
                    lastVertex0 = newVertex0;
                    lastVertex1 = newVertex1;
                    //dragging = false;
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
        //    Debug.LogError(++count + " : " + Vector3.Distance(point0, previousSpine.GetComponent<TearChank>().originalVertexPosition0));
        //    float distance = Vector3.Distance(point0, previousSpine.GetComponent<TearChank>().originalVertexPosition0);
        //    if(distance > draggingthreshold)
        //    {
        //        Vector3 newp0 = point0 * Time.deltaTime;
        //        Vector3 newp1 = point1 * Time.deltaTime;
        //        CreateVertexSpine(newp0, newp1);
        //    }
        //}

        GameObject centerVertex = new GameObject("VertexCenter");
        centerVertex.transform.localPosition = (point0 + point1) / 2;
        tearMeshPart.transform.InverseTransformPoint(point0);
        tearMeshPart.transform.InverseTransformPoint(point1);

        GameObject vertex0 = new GameObject("point0");
        GameObject vertex1 = new GameObject("point1");
        vertex0.transform.localPosition = point0;
        vertex1.transform.localPosition = point1;

        vertex0.transform.parent = centerVertex.transform;
        vertex1.transform.parent = centerVertex.transform;

        TearChank latestTearChank = centerVertex.AddComponent<TearChank>();
        latestTearChank.vertex0 = vertex0;
        latestTearChank.vertex1 = vertex1;

        if (previousSpine == null)
        {
            centerVertex.transform.parent = vertexSpinePart.transform;
        }
        else
        {
            previousSpine.transform.parent = centerVertex.transform;
            previousSpine.transform.Rotate(point0 - point1, foldingAngle);

            TearChank previousTearChank = previousSpine.GetComponent<TearChank>();
            latestTearChank.previousTearChank = previousTearChank;

            ClearNewChankVertices(point0, point1, previousTearChank.originalVertexPosition0, previousTearChank.originalVertexPosition1);
        }

        latestTearChank.originalVertexPosition0 = point0;
        latestTearChank.originalVertexPosition1 = point1;
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
        Destroy(newChankMesh);
        _mesh.colors = originalMeshColors;
    }

    #endregion ALL SELF DECLEAR FUNCTIONS

}
