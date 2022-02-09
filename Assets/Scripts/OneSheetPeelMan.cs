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
    public float cuttingSize = 5;
    public float draggingthreshold = 0.2f;
    public Material tearPartMaterial;

    Vector3 startPoint, endPoint, previousDraggingPosition;
    bool dragging, initialVertexCreated;
    string outputText = "Did not start";

    GameObject tearPart;
    Mesh mesh;
    MeshFilter meshFilter;
    Vector3[] vertices;
    int[] triangle;
    Vector3 newVertex0, newVertex1, lastVertex0, lastVertex1;

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

                tearPart = new GameObject("Tear Part");
                tearPart.transform.position = previousDraggingPosition.ModifyThisVector(0, 0.01f, 0);

                meshFilter = tearPart.AddComponent<MeshFilter>();
                tearPart.AddComponent<MeshRenderer>().material = tearPartMaterial;
                mesh = new Mesh();
                meshFilter.mesh = mesh;
                vertices = new Vector3[] { };
                triangle = new int[] { };
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
                    Vector3 side1 = previousDraggingPosition - raycastHit.point;
                    Vector3 side2 = raycastHit.normal;
                    Vector3 tangentDir = Vector3.Cross(side1, side2).normalized * cuttingSize;
                    if (!initialVertexCreated)
                    {
                        lastVertex0 = previousDraggingPosition + tangentDir;
                        lastVertex1 = previousDraggingPosition - tangentDir;
                        Array.Resize(ref vertices, vertices.Length + 2);
                        vertices[vertices.Length - 2] = lastVertex0;
                        vertices[vertices.Length - 1] = lastVertex1;

                        Debug.DrawLine(previousDraggingPosition, raycastHit.point, Color.blue, 10);
                        Debug.DrawRay(previousDraggingPosition, tangentDir, Color.red, 10);
                        Debug.DrawRay(previousDraggingPosition, -tangentDir, Color.red, 10);

                        initialVertexCreated = true;
                    }

                    newVertex0 = raycastHit.point + tangentDir;
                    newVertex1 = raycastHit.point - tangentDir;

                    Array.Resize(ref vertices, vertices.Length + 2);
                    vertices[vertices.Length - 2] = newVertex0;
                    vertices[vertices.Length - 1] = newVertex1;

                    Array.Resize(ref triangle, triangle.Length + 6);
                    triangle[triangle.Length - 6] = vertices.Length - 4;
                    triangle[triangle.Length - 5] = vertices.Length - 3;
                    triangle[triangle.Length - 4] = vertices.Length - 2;
                    triangle[triangle.Length - 3] = vertices.Length - 3;
                    triangle[triangle.Length - 2] = vertices.Length - 1;
                    triangle[triangle.Length - 1] = vertices.Length - 2;

                    Debug.DrawRay(raycastHit.point, tangentDir, Color.red, 10);
                    Debug.DrawRay(raycastHit.point, -tangentDir, Color.red, 10);


                    mesh.vertices = vertices;
                    mesh.triangles = triangle;
                    Debug.LogError(triangle.Length);
                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                    mesh.RecalculateTangents();
                    Debug.LogError(triangle.Length);

                    for (int i = 0; i < vertices.Length; i++)
                    {
                        GameObject go = new GameObject("Vertex_" + i);
                        go.transform.position = vertices[i];
                    }

                    previousDraggingPosition = raycastHit.point;
                    lastVertex0 = newVertex0;
                    lastVertex1 = newVertex1;
                    dragging = false;
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


    #endregion ALL SELF DECLEAR FUNCTIONS

}
