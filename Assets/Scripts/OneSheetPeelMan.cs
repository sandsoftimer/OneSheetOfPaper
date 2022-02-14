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
    public Material tearPartMaterial;

    Vector3 startPoint, endPoint, previousDraggingPosition;
    bool dragging, initialVertexCreated;
    string outputText = "Did not start";

    GameObject tearPart, vertexSpinePart, previousSpine;
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
                tearPart.transform.position = previousDraggingPosition;

                vertexSpinePart = new GameObject("Vertex Spine Part");
                vertexSpinePart.transform.position = previousDraggingPosition;

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
                        lastVertex0 = transform.TransformPoint(previousDraggingPosition + tangentDir);
                        lastVertex1 = transform.TransformPoint(previousDraggingPosition - tangentDir);

                        Array.Resize(ref vertices, vertices.Length + 2);
                        CreateVertexSpine(lastVertex0, lastVertex1);
                        //vertices[vertices.Length - 2] = lastVertex0;
                        //vertices[vertices.Length - 1] = lastVertex1;

                        //Debug.DrawLine(previousDraggingPosition, raycastHit.point, Color.blue, 10);
                        //Debug.DrawRay(previousDraggingPosition, tangentDir, Color.red, 1000);
                        //Debug.DrawRay(previousDraggingPosition, -tangentDir, Color.red, 1000);

                        initialVertexCreated = true;
                    }

                    newVertex0 = transform.TransformPoint(raycastHit.point + tangentDir);
                    newVertex1 = transform.TransformPoint(raycastHit.point - tangentDir);

                    Array.Resize(ref vertices, vertices.Length + 2);
                    CreateVertexSpine(newVertex0, newVertex1);
                    //vertices[vertices.Length - 2] = newVertex0;
                    //vertices[vertices.Length - 1] = newVertex1;

                    Array.Resize(ref triangle, triangle.Length + 6);
                    triangle[triangle.Length - 6] = vertices.Length - 4;
                    triangle[triangle.Length - 5] = vertices.Length - 3;
                    triangle[triangle.Length - 4] = vertices.Length - 2;
                    triangle[triangle.Length - 3] = vertices.Length - 3;
                    triangle[triangle.Length - 2] = vertices.Length - 1;
                    triangle[triangle.Length - 1] = vertices.Length - 2;

                    //GameObject vertexCenter = previousSpine.transform.GetChild(2).gameObject;
                    //do
                    //{
                    //    Array.Resize(ref triangle, triangle.Length + 6);
                    //    triangle[triangle.Length - 6] = vertices.Length - 4;
                    //    triangle[triangle.Length - 5] = vertices.Length - 3;
                    //    triangle[triangle.Length - 4] = vertices.Length - 2;
                    //    triangle[triangle.Length - 3] = vertices.Length - 3;
                    //    triangle[triangle.Length - 2] = vertices.Length - 1;
                    //    triangle[triangle.Length - 1] = vertices.Length - 2;

                    //} while (vertexCenter.transform.GetChild(2).childCount > 2);


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

    void CreateVertexSpine(Vector3 point0, Vector3 point1)
    {
        GameObject vc = new GameObject("VertexCenter");
        vc.transform.localPosition = (point0 + point1) / 2;
        tearPart.transform.InverseTransformPoint(point0);
        tearPart.transform.InverseTransformPoint(point1);

        GameObject p0 = new GameObject("point0");
        GameObject p1 = new GameObject("point1");
        p0.transform.localPosition = point0;
        p1.transform.localPosition = point1;
        p0.transform.parent = vc.transform;
        p1.transform.parent = vc.transform;

        if (previousSpine == null)
        {
            vc.transform.parent = vertexSpinePart.transform;
        }
        else
        {
            previousSpine.transform.parent = vc.transform;
            previousSpine.transform.Rotate(point0 - point1, foldingAngle);
        }
        vertices[vertices.Length - 2] = tearPart.transform.InverseTransformPoint(p0.transform.position);
        vertices[vertices.Length - 1] = tearPart.transform.InverseTransformPoint(p1.transform.position);
        previousSpine = vc;
    }

    #endregion ALL SELF DECLEAR FUNCTIONS

}
