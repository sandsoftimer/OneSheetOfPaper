using UnityEngine;

//[System.Serializable]
public class MeshUVPainter
{   
    public MeshRenderer meshRenderer;
    public MeshFilter filter;
    public Collider lastCollider;

    public Material material;
    public Texture2D texture;
    public Mesh mesh;

    Vector3[] vertices;
    int[] triangleIndexes;
    Color[] pixelBuffer, pixelBufferWithOffset;
    Color oldColor;

    public int vCount;
    public float progress;
    float d;
    float f;
    float delta;
    public float fl;
    public int count;
    float[] progVerts;

    public UVPaintOutput PaintOnUV(RaycastHit raycast,RaycastHit prevhit, Color color, float radius, float sharpness,  Vector2 rectangle)
    {
        //if (lastCollider != raycast.collider)
        //{
        //    InitializeNewMesh(raycast.collider);
        //    lastCollider = raycast.collider;
        //}

        delta = Time.deltaTime;
        Vector2 uv = raycast.textureCoord * ((texture.width) - 1);
        Vector2 preuv= prevhit.textureCoord * ((texture.width) - 1);
        if (uv.x == preuv.x && uv.y == preuv.y)
            return new UVPaintOutput() { texture = this.texture, pixelBufferWithOffset = this.pixelBufferWithOffset };
        Vector2 dir = (uv - preuv).normalized;
        float dist = (uv - preuv).magnitude;
        float oldist = fl;
        fl += dist;
        count++;
        Vector2 midvec = (uv + preuv) / 2;
        for (int u = 0; u < (texture.width); u++)
        {
            for (int v = 0; v < (texture.width); v++)
            {
                float distSqr = (u - uv.x) * (u - uv.x) + (v - uv.y) * (v - uv.y);
                float newdistSqr = (u - preuv.x) * (u - preuv.x) + (v - preuv.y) * (v - preuv.y);
                Vector2 newv = new Vector2(((u - midvec.x) * dir.y) + (v - midvec.y) * (-dir.x), ((u - midvec.x) * dir.x) + (v - midvec.y) * (dir.y));

                if (((newv.x < rectangle.x / 2) && (newv.x > -rectangle.x / 2) && (newv.y < dist / 2) && (newv.y > -dist / 2))||(newdistSqr<((rectangle.x*rectangle.x)/4)&& (newv.y < -dist / 2) && (newv.y > -(dist / 2)-oldist)))
                {
                    d = Mathf.Sqrt(distSqr);
                    f = sharpness * delta * ((radius - d) / radius);
                    oldColor = pixelBuffer[v * (texture.width) + u];
                    if (oldColor.a != 0)
                    {
                        oldColor.r = Mathf.Lerp(oldColor.r, color.r, f);
                        oldColor.g = Mathf.Lerp(oldColor.g, color.g, f);
                        oldColor.b = Mathf.Lerp(oldColor.b, color.b, f);
                        oldColor.a = Mathf.Clamp01(oldColor.a + f);
                    }
                    else
                    {
                        oldColor = color;
                        oldColor.a = f;
                    }
                    oldColor.a = 0;

                    pixelBuffer[v * (texture.width) + u] = oldColor;
                }
                Vector2 newRect = rectangle;
                newRect.x -= Random.Range(5f, 10);
                newv.y -= Random.Range(5f, 10);
                if (((newv.x < newRect.x / 2) && (newv.x > -newRect.x / 2) && (newv.y < dist / 2) && (newv.y > -dist / 2)) || (newdistSqr < ((newRect.x * newRect.x) / 4) && (newv.y < -dist / 2) && (newv.y > -(dist / 2) - oldist)))
                {
                    oldColor = pixelBufferWithOffset[v * (texture.width) + u];
                    if (oldColor.a != 0)
                    {
                        oldColor.r = Mathf.Lerp(oldColor.r, color.r, f);
                        oldColor.g = Mathf.Lerp(oldColor.g, color.g, f);
                        oldColor.b = Mathf.Lerp(oldColor.b, color.b, f);
                        oldColor.a = Mathf.Clamp01(oldColor.a + f);
                    }
                    else
                    {
                        oldColor = color;
                        oldColor.a = f;
                    }
                    oldColor.a = 0;

                    pixelBufferWithOffset[v * (texture.width) + u] = oldColor;
                }
            }
        }
        texture.SetPixels(pixelBuffer);
        texture.Apply();

        return new UVPaintOutput() { texture = this.texture, pixelBufferWithOffset = this.pixelBufferWithOffset };
    }

    public void InitializeNewMesh(Material material)
    {
        //meshRenderer = collider.GetComponent<MeshRenderer>();
        //filter = collider.GetComponent<MeshFilter>();

        this.material = material;
        //mesh = filter.mesh;
        //triangleIndexes = new int[mesh.triangles.Length];
        //vCount = filter.mesh.vertexCount;
        //vertices = filter.mesh.vertices;
        //progVerts = new float[vCount];
        //progress = 0;

        texture = MonoBehaviour.Instantiate(this.material.mainTexture as Texture2D);
        pixelBuffer = texture.GetPixels();
        pixelBufferWithOffset = texture.GetPixels();
        this.material.SetTexture("_BaseMap", texture);
    }
}

public class UVPaintOutput
{
    public Texture2D texture;
    public Color[] pixelBufferWithOffset;
}
