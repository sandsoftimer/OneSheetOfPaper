using System;
using UnityEngine;

[Serializable]
public class AnimationData
{
    public TextureSequence[] textureSequences;
}

[Serializable]
public class TextureSequence
{
    public float fps = 4;
    public int loopCount = 0;
    public Texture2D[] textures;
}

[Serializable]
public class DependencyData
{
    public PaperTearPart dependen_part;
    [Range(0, 100)]
    public float dragLimit = 50f;
}

public class TriangleData
{
    public Vector3 v0, v1, v2;
    public int triangleIndex;

    public Vector3 GetCenter()
    {
        return (v0 + v1 + v2) / 3;
    }
}