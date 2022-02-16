using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.AlphaPotato.Utility
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshDoctor : APBehaviour
    {
        public bool createPlane;
        public bool addCollider;
        public int planeXSize, planeZSize;
        public float planeGap = 1;
        public Material planeMaterial;
        public Pivot pivot;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;
        Mesh mesh;
        Dictionary<Vector3, int[]> vertexTriangleIndexes; 

        public Vector3[] originalVertices { get { return mesh.vertices; } }
        public int[] originalTriangles { get { return mesh.triangles; } }
        public Vector2[] originalUVs { get { return mesh.uv; } }

        #region ALL UNITY FUNCTIONS

        // Awake is called before Start
        public override void Awake()
        {
            base.Awake();

            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
            mesh = meshFilter.mesh;

            //planeMaterial.SetTextureScale("_BaseTex", new Vector2(planeSixe.x, planeSixe.z));

            if (createPlane)
            {
                CreatePlane();
            }
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

        }

        #endregion ALL UNITY FUNCTIONS
        //=================================   
        #region ALL OVERRIDING FUNCTIONS


        #endregion ALL OVERRIDING FUNCTIONS
        //=================================
        #region ALL SELF DECLEAR FUNCTIONS

        public List<Vector3> GetClosestVertices(Vector3 point, float radious)
        {
            List<Vector3> closestVertices = new List<Vector3>();



            return closestVertices;
        }

        public Vector3[] GetVerticesCopy()
        {
            Vector3[] copyVerts = new Vector3[originalVertices.Length];

            for (int i = 0; i < originalVertices.Length; i++)
            {
                copyVerts[i] = originalVertices[i];
            }
            return copyVerts;
        }

        public int[] GetTrianglesCopy()
        {
            int[] copyTris = new int[originalTriangles.Length];

            for (int i = 0; i < originalVertices.Length; i++)
            {
                copyTris[i] = originalTriangles[i];
            }
            return copyTris;
        }

        public Mesh GetMesh()
        {
            return mesh;
        }

        public void UpdateMesh(Vector3[] verts, int[] tris, Vector2[] uvs)
        {
            mesh.vertices = verts;
            mesh.triangles = tris;
            //mesh.uv = uvs;

            if (meshFilter != null)
            {
                mesh.RecalculateTangents();
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                meshFilter.GetComponent<MeshCollider>().sharedMesh = mesh;
            }
        }

        public void RecalculateMesh()
        {
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        public virtual void CreateEditorPlane()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

            if (addCollider && meshCollider != null)
            {
                DestroyImmediate(meshCollider);
            }

            CreatePlane();
        }

        public void CreatePlane()
        {
            mesh = new Mesh() { name = gameObject.name };

            mesh.vertices = GenarateVerticies();
            mesh.triangles = GenarateTriangles();
            mesh.uv = GenarateUVs();

            if(planeMaterial != null)
                meshRenderer.material = planeMaterial;

            if (addCollider)
            {
                if (meshCollider == null)
                {
                    meshCollider = gameObject.AddComponent<MeshCollider>();
                }
                meshCollider.sharedMesh = mesh;
            }

            meshFilter.mesh = mesh;
        }

        Vector3[] GenarateVerticies()
        {
            Vector3[] verts = new Vector3[(planeXSize + 1) * (planeZSize + 1)];

            Vector3 offset = Vector3.zero;
            switch (pivot)
            {
                case Pivot.TOP_LEFT:
                    offset.x = 0;
                    offset.y = 0;
                    offset.z = -planeZSize;
                    break;
                case Pivot.TOP_CENTER:
                    offset.x = -planeXSize / 2f;
                    offset.y = 0;
                    offset.z = -planeZSize;
                    break;
                case Pivot.TOP_RIGHT:
                    offset.x = -planeXSize;
                    offset.y = 0;
                    offset.z = -planeZSize;
                    break;
                case Pivot.MIDDLE_LEFT:
                    offset.x = 0;
                    offset.y = 0;
                    offset.z = -planeZSize / 2f;
                    break;
                case Pivot.MIDDLE_CENTER:
                    offset.x = -planeXSize / 2f;
                    offset.y = 0;
                    offset.z = -planeZSize / 2f;
                    break;
                case Pivot.MIDDLE_RIGHT:
                    offset.x = -planeXSize;
                    offset.y = 0;
                    offset.z = -planeZSize / 2f;
                    break;
                case Pivot.BOTTOM_LEFT:
                    offset.x = 0;
                    offset.y = 0;
                    offset.z = 0;
                    break;
                case Pivot.BOTTOM_CENTER:
                    offset.x = -planeXSize / 2f;
                    offset.y = 0;
                    offset.z = 0;
                    break;
                case Pivot.BOTTOM_RIGHT:
                    offset.x = -planeXSize;
                    offset.y = 0;
                    offset.z = 0;
                    break;
            }


            for (int i = 0, z = 0; z <= planeZSize; z++)
            {
                for (int x = 0; x <= planeXSize; x++)
                {
                    verts[i++] = new Vector3((x + offset.x) * planeGap, 0, (z + offset.z) * planeGap);
                }
            }
            return verts;
        }

        int[] GenarateTriangles()
        {
            int[] tries = new int[planeXSize * planeZSize * 6];
            int vertIndex = 0;
            int triIndex = 0;
            for (int z = 0; z < planeZSize; z++)
            {
                for (int x = 0; x < planeXSize; x++)
                {
                    tries[triIndex + 0] = vertIndex + 0;
                    tries[triIndex + 1] = vertIndex + planeXSize + 1;
                    tries[triIndex + 2] = vertIndex + 1;
                    tries[triIndex + 3] = vertIndex + 1;
                    tries[triIndex + 4] = vertIndex + planeXSize + 1;
                    tries[triIndex + 5] = vertIndex + planeXSize + 2;

                    SetVertexTriangleIndexes(vertIndex + 0, triIndex);
                    SetVertexTriangleIndexes(vertIndex + planeXSize + 1, triIndex + 1);
                    SetVertexTriangleIndexes(vertIndex + 1, triIndex + 2);
                    SetVertexTriangleIndexes(vertIndex + 1, triIndex + 3);
                    SetVertexTriangleIndexes(vertIndex + planeXSize + 1, triIndex + 4);
                    SetVertexTriangleIndexes(vertIndex + planeXSize + 2, triIndex + 5);


                    vertIndex++;
                    triIndex += 6;
                }
                vertIndex++;
            }
            return tries;
        }

        Vector2[] GenarateUVs()
        {
            Vector2[] uvs = new Vector2[(planeXSize + 1) * (planeZSize + 1)];
            int i = 0;
            for (int z = 0; z <= planeZSize; z += 1)
            {
                for (int x = 0; x <= planeXSize; x += 1)
                {
                    uvs[i] = new Vector2((float)x / planeXSize, (float)z / planeZSize);
                    i++;
                }
            }
            return uvs;
        }

        void SetVertexTriangleIndexes(int vertIndex, int triIndex)
        {
            if(vertexTriangleIndexes == null)
                vertexTriangleIndexes = new Dictionary<Vector3, int[]>();

            if (vertexTriangleIndexes.ContainsKey(originalVertices[vertIndex]))
            {
                int[] existingTriangleIndexes = vertexTriangleIndexes[originalVertices[vertIndex]];
                int[] newTriangleIndexes = new int[existingTriangleIndexes.Length + 1];
                newTriangleIndexes[newTriangleIndexes.Length - 1] = triIndex;
                vertexTriangleIndexes[originalVertices[vertIndex]] = newTriangleIndexes;
            }
            else
            {
                int[] newTriangleIndexes = new int[1];
                newTriangleIndexes[0] = triIndex;
                vertexTriangleIndexes[originalVertices[vertIndex]] = newTriangleIndexes;
            }
        }

        public int[] GetVertexTriangleIndexes(Vector3 vertex)
        {
            int[] triangleIndexes;

            triangleIndexes = vertexTriangleIndexes[vertex];
            return triangleIndexes;
        }

        public void DeleteTriangle(int triangleIndex, int[] tris = null)
        {
            if (tris == null) tris = originalTriangles;
            Array.Clear(tris, triangleIndex * 3, 3);
            UpdateMesh(originalVertices, tris, originalUVs);
        }

        public void PlaceTriangleToZero(int triangleIndex)
        {
            int[] tris = originalTriangles;
            tris[triangleIndex * 3 + 0] = 0;
            tris[triangleIndex * 3 + 1] = 0;
            tris[triangleIndex * 3 + 2] = 0;
            UpdateMesh(originalVertices, tris, originalUVs);
        }

        public void SubDivideTriangleWithVertex(int triangleIndex, Vector3 newVertex, float jointVerticesInfluence)
        {
            int[] tris = originalTriangles;
            int v0Index, v1Index, v2Index;
            v0Index = tris[triangleIndex * 3];
            v1Index = tris[triangleIndex * 3 + 1];
            v2Index = tris[triangleIndex * 3 + 2];
            DeleteTriangle(triangleIndex, tris);

            Vector3[] verts = originalVertices;
            Array.Resize(ref verts, verts.Length + 1);
            verts[verts.Length - 1] = meshFilter.transform.InverseTransformPoint(newVertex);

            Array.Resize(ref tris, tris.Length + 3);
            tris[tris.Length - 3] = v1Index;
            tris[tris.Length - 2] = verts.Length - 1;
            tris[tris.Length - 1] = v0Index;

            float influenceY = verts[v0Index].y + jointVerticesInfluence > newVertex.y ? verts[v0Index].y : verts[v0Index].y + jointVerticesInfluence;
            verts[v0Index] = new Vector3(verts[v0Index].x,
                influenceY,
                verts[v0Index].z);

            Array.Resize(ref tris, tris.Length + 3);
            tris[tris.Length - 3] = verts.Length - 1;
            tris[tris.Length - 2] = v1Index;
            tris[tris.Length - 1] = v2Index;

            influenceY = verts[v1Index].y + jointVerticesInfluence > newVertex.y ? verts[v1Index].y : verts[v1Index].y + jointVerticesInfluence;
            verts[v1Index] = new Vector3(verts[v1Index].x,
                influenceY,
                verts[v1Index].z);

            Array.Resize(ref tris, tris.Length + 3);
            tris[tris.Length - 3] = v0Index;
            tris[tris.Length - 2] = verts.Length - 1;
            tris[tris.Length - 1] = v2Index;

            influenceY = verts[v2Index].y + jointVerticesInfluence > newVertex.y ? verts[v2Index].y : verts[v2Index].y + jointVerticesInfluence;
            verts[v2Index] = new Vector3(verts[v2Index].x,
                influenceY,
                verts[v2Index].z);

            UpdateMesh(verts, tris, originalUVs);
        }

        public void InfluenceTriangle(int triangleIndex, Vector3 triangleNormal, float influenceRate)
        {
            Vector3[] verts = originalVertices;
            int[] tris = originalTriangles;

            int v0Index, v1Index, v2Index;
            v0Index = tris[triangleIndex * 3];
            v1Index = tris[triangleIndex * 3 + 1];
            v2Index = tris[triangleIndex * 3 + 2];

            verts[v0Index] += triangleNormal * influenceRate;
            verts[v1Index] += triangleNormal * influenceRate;
            verts[v2Index] += triangleNormal * influenceRate;

            UpdateMesh(verts, tris, originalUVs);
        }


        public void PaintMesh(Vector3 point, Color color, float brushSize)
        {
            Vector3[] vertpos = mesh.vertices;
            Color[] meshcolors = mesh.colors;
            if (meshcolors.Length == 0)
            {
                meshcolors = new Color[mesh.vertices.Length];
                for (int i = 0; i < meshcolors.Length; i++)
                {
                    meshcolors[i] = Color.white;
                }
            }

            for (int i = 0; i < vertpos.Length; i++)
            {
                if ((point - transform.TransformPoint(vertpos[i])).sqrMagnitude < brushSize)
                {
                    meshcolors[i] = color;
                }
            }
            mesh.colors = meshcolors;
        }

        public void _PaintMesh(RaycastHit hit, Color color, float brushSize)
        {
            Debug.LogError("Coloring");
            Vector3[] vertpos = mesh.vertices;
            Color[] meshcolors = mesh.colors;
            if (meshcolors.Length == 0)
            {
                meshcolors = new Color[mesh.vertices.Length];
                for (int i = 0; i < meshcolors.Length; i++)
                {
                    meshcolors[i] = Color.white;
                }
            }

            for (int i = 0; i < vertpos.Length; i++)
            {
                if ((hit.point - hit.transform.TransformPoint(vertpos[i])).sqrMagnitude < brushSize)
                {
                    meshcolors[i] = color;
                }
            }
            mesh.colors = meshcolors;
        }

        #endregion ALL SELF DECLEAR FUNCTIONS

    }
}