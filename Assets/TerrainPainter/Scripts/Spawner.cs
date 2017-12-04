using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public String ParentName;
    public LayerMask groundLayer;
    
    [Range(1, 25)]
    public int Instances = 1;
    [Range(1, 100)]
    public float SpawnRange = 1;
    public Shape SpawnShape;
    
    public List<SpawnerAsset> Assets = new List<SpawnerAsset>();
    
    public enum Shape
    {
        Circle,
        Square
    }
}
