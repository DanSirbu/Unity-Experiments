using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
	public GameObject groundPrefab;
	public int size = 10;
	public LayerMask treeLayer;
	public LayerMask rockLayer;

	public GameObject[] waterTiles = new GameObject[16];
	public GameObject[] treeTiles = new GameObject[0];
	public GameObject[] rockTiles = new GameObject[2];
}
