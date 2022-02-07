using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Com.AlphaPotato.Utility;

public class OneSheetPeelMan : APBehaviour
{
    public MeshDoctor meshDoctor;
    public TextMeshProUGUI text;

    [Header("Level Rules")]
    public float cuttingSize = 5;

    Vector3 startPoint, endPoint, previousDraggingPosition;
    bool dragging;
    string outputText = "Did not start";

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
            Cut();
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

        if (dragging)
            previousDraggingPosition = APTools.mathManager.GetWorldTouchPosition(Input.mousePosition);
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

    void Cut()
    {
        Vector3[] verts = meshDoctor.originalVertices;
        int horizontalCenter = meshDoctor.planeXSize / 2;
        int verticalCenter = meshDoctor.planeZSize / 2;

        int topEdge = 0, bottomEdge = 0;
        for (int z = 0; z <= cuttingSize / 2; z++)
        {
            for (int x = 0; x <= horizontalCenter; x++)
            {
                verts[(verticalCenter + z) * (meshDoctor.planeXSize + 1) + x] += new Vector3(0, 1, 0);
                verts[(verticalCenter - (z + 1)) * (meshDoctor.planeXSize + 1) + x] += new Vector3(0, 1, 0);
            }
            topEdge = verticalCenter + z + 1;
            bottomEdge = verticalCenter - (z + 2);
        }
        for (int x = 0; x <= horizontalCenter; x++)
        {
            verts[topEdge * (meshDoctor.planeXSize + 1) + x] += new Vector3(0, -1, 0);
            verts[bottomEdge * (meshDoctor.planeXSize + 1) + x] += new Vector3(0, -1, 0);
        }

        meshDoctor.UpdateMesh(verts, meshDoctor.originalTriangles, meshDoctor.originalUVs);
        //int name = 1;
        //for (int z = 0; z <= meshDoctor.planeZSize; z++)
        //{
        //    for (int x = 0; x <= meshDoctor.planeXSize; x++)
        //    {
        //        GameObject go = new GameObject(name.ToString());
        //        go.transform.position = verts[z * (meshDoctor.planeZSize + 1) + x];
        //        go.transform.parent = transform;
        //        name++;
        //    }
        //}
    }

    #endregion ALL SELF DECLEAR FUNCTIONS

}
