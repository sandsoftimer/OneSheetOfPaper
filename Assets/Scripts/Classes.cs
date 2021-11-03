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
    public float fps;
    public int loopCount;
    public Texture2D[] textures;
}