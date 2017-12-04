using System;
using UnityEngine;

[Serializable]
public class SpawnerAsset
{
    [Range(0, 1)]
    public float SuccessRate = 1.0f;

    public GameObjectVisual Asset;
}
