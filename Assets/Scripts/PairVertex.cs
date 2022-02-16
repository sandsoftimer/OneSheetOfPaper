using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PairVertex : MonoBehaviour
{
    public PairVertex previousPairVertex;
    public GameObject vertexObject0, vertexObject1;
    public Vector3 vertexPosition0, vertexPosition1;

    public void SetVertex(GameObject vertexObject0, GameObject vertexObject1, Vector3 vertexPosition0, Vector3 vertexPosition1)
    {
        this.vertexObject0 = vertexObject0;
        this.vertexObject1 = vertexObject1;
        this.vertexPosition0 = vertexPosition0;
        this.vertexPosition1 = vertexPosition1;
        vertexObject0.transform.position = vertexPosition0;
        vertexObject1.transform.position = vertexPosition1;
    }
}
