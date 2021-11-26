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