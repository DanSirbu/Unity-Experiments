using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(TerrainGen))]
public class TerrainGenEditor : Editor
{
    private SerializedObject obj;
    private SerializedProperty groundPrefab;
    
    private LayerMask groundLayerIndex;
    private LayerMask groundLayerValue;
    private bool leftPressed;

    private float paintSize = 1;
    private float paintNumObjects = 5f;

    private float objSize;

    private GameObject ground;
    private TerrainGen _terrainGen;
    private int terrainSize;
    private TerrainGen mTerrainGen;
    private GameObject[,] terrainTiles;
    private Vector3 previousMouseEventLocation = Vector3.positiveInfinity;

    private GameObject treeParent;
    private GameObject rockParent;
    
    public enum DrawState
    {
        ADDWATER,
        ADDTREES,
        ADDROCKS
    }

    public static DrawState state = DrawState.ADDWATER;

    public Dictionary<string, Pair<bool, Action>> button = new Dictionary<string, Pair<bool, Action>>()
    {
        { "Water", Tuples.pair<bool, Action>(false, () => state = DrawState.ADDWATER)},
        { "Trees", Tuples.pair<bool, Action>(false, () => state = DrawState.ADDTREES)},
        { "Rocks", Tuples.pair<bool, Action>(false, () => state = DrawState.ADDROCKS)}
    };
    public void OnEnable()
    {
        treeParent = GameObject.Find("Trees");
        rockParent = GameObject.Find("Rocks");
        
        obj = new SerializedObject(target);
        groundPrefab = obj.FindProperty("groundPrefab");
        terrainSize = obj.FindProperty("size").intValue;
        _terrainGen = target as TerrainGen;
        
        var groundPrefabRef = groundPrefab.GetValue<GameObject>();
        objSize = groundPrefabRef == null ? 0 : groundPrefabRef.GetComponentInChildren<Renderer>().bounds.size.x;
        groundLayerIndex = LayerMask.NameToLayer("Ground");
        groundLayerValue = LayerMask.GetMask("Ground");
        ground = GameObject.Find("Ground");
        
        ReloadTerrain();
    }

    private void ReloadTerrain()
    {
        if(ground == null) return;
        terrainTiles = new GameObject[terrainSize, terrainSize];
        
        for (int i = 0; i < ground.transform.childCount; i++)
        {
            var instantiatedObject = ground.transform.GetChild(i).gameObject;
            var nonRoundedX = instantiatedObject.transform.position.x / objSize;
            var nonRoundedY = instantiatedObject.transform.position.z / objSize;

            var x = Mathf.CeilToInt(nonRoundedX);
            var y = Mathf.CeilToInt(nonRoundedY);
            
            terrainTiles[x, y] = instantiatedObject;
        }
    }

    public void OnMakeWaterTile(int positionX, int positionY)
    {
        //http://www.angryfishstudios.com/2011/04/adventures-in-bitmasking/
        
        //Update Center First
        UpdateWaterTile(positionX, positionY, CalculateWaterCode(positionX, positionY));
        
        //Then update all tiles that touch center
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if(x == 0 && y == 0) continue;

                var newX = positionX + x;
                var newY = positionY + y;
                if (isWater(newX, newY))
                {
                    //Update Left,Right,Top,Bottom
                    UpdateWaterTile(newX, newY, CalculateWaterCode(newX, newY));
                }
            }
        }
    }

    private int CalculateWaterCode(int positionX, int positionY)
    {
        int code = 0;
        //TOP
        if (isWater(positionX, positionY - 1))
        {
            code += 1;
        }
        //RIGHT
        if (isWater(positionX - 1, positionY))
        {
            code += 2;
        }
        //BOTTOM
        if (isWater(positionX, positionY + 1))
        {
            code += 4;
        }
        //LEFT
        if (isWater(positionX + 1, positionY))
        {
            code += 8;
        }
        return code;
    }
    private void UpdateWaterTile(int x, int y, int code)
    {
        //Debug.Log("Updating water at " + x + "," + y + " with " + code);
        Undo.DestroyObjectImmediate(terrainTiles[x, y]);
        var waterPrefab = _terrainGen.waterTiles[code];
        var instantiatedObject = Instantiate(waterPrefab, XYToWorldSpace(x, y), Quaternion.identity, ground.transform);
        Undo.RegisterCreatedObjectUndo(instantiatedObject, "Water Tile");
        terrainTiles[x, y] = instantiatedObject;
    }

    private Vector3 XYToWorldSpace(int x, int y)
    {
        return new Vector3(x * objSize, 0, y * objSize);
    }

    private int[] WorldSpaceToXY(Vector3 worldVector)
    {
        var realX = Mathf.FloorToInt(Mathf.FloorToInt(worldVector.x + objSize) / objSize);
        var realY = Mathf.FloorToInt(Mathf.FloorToInt(worldVector.z + objSize) / objSize);
        
        return new[] { realX, realY };
    }
    private bool isWater(int x, int y)
    {
        //Out of bounds
        if (x < 0 || y < 0 || x >= terrainSize || y >= terrainSize)
            return false;

        return terrainTiles[x, y].name.Contains("Water");
    }

    public void OnDisable()
    {
        //Debug.Log("OnDisable Terrain");
        //SceneView.onSceneGUIDelegate -= this.OnAlwaysSceneGUI;
    }
    
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            var groundPrefabRef = groundPrefab.GetValue<GameObject>();
            var terrain = target as TerrainGen;
            terrainSize = terrain.size;
            
            ground = GameObject.Find("Ground");
            if(ground != null)
                Undo.DestroyObjectImmediate(ground);

            ground = new GameObject("Ground") {layer = groundLayerIndex};

            Undo.RegisterCreatedObjectUndo(ground, "Create Ground");
            
            objSize = groundPrefabRef.GetComponentInChildren<Renderer>().bounds.size.x;
            
            for (int x = 0; x < terrain.size; x++)
            {
                for (int y = 0; y < terrain.size; y++)
                {
                    var position = new Vector3(x * objSize, 0, y * objSize);
                    var instantiatedObject = Instantiate(groundPrefabRef, position, Quaternion.identity, ground.transform);
                    instantiatedObject.layer = groundLayerIndex;

                    for (int i = 0; i < instantiatedObject.transform.childCount; i++)
                    {
                        instantiatedObject.transform.GetChild(0).gameObject.layer = groundLayerIndex;
                    }
                }
            }
            var boxCollider = ground.AddComponent<BoxCollider>();
            var centerXY = (objSize * terrainSize) / 2 - objSize;
            boxCollider.center = new Vector3(centerXY, 0, centerXY);
            boxCollider.size = new Vector3(objSize * terrainSize, 0.3f, objSize * terrainSize);
            
            ReloadTerrain();
        }
        
        EditorGUILayout.BeginHorizontal ();
        foreach (var buttonKey in button.Keys)
        {
            var buttonVal = button[buttonKey];
            var initialValue = buttonVal.first;
            buttonVal.first = GUILayout.Toggle(buttonVal.first, buttonKey, EditorStyles.miniButton);
            if (buttonVal.first && initialValue != buttonVal.first)
            {
                buttonVal.second.Invoke();
                foreach (var buttonValue in button.Values)
                {
                    buttonValue.first = false;
                }
                buttonVal.first = true;
            }
        }
        EditorGUILayout.EndHorizontal ();
        
        GUILayout.Label("Num Objects Per Paint");
        paintNumObjects = GUILayout.HorizontalSlider(paintNumObjects, 1, 100);
        
        DrawDefaultInspector();
        
        obj.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, groundLayerValue))
        {
            OnHoverOverGround(hit.point);
        }
        var terrainGen = target as TerrainGen;
        
        Handles.color = Color.black;
        for (int x = -1; x < terrainGen.size; x++)
        {
            var start1 = x * objSize;
            var start2 = -1 * objSize;
            var end1 = start1;
            var end2 = (terrainGen.size - 1) * objSize;
            var offset = 0.35f;
            
            //Horizontal Lines
            Handles.DrawLine(new Vector3(start1, offset, start2), new Vector3(end1, offset, end2));
            //Vertical Lines
            Handles.DrawLine(new Vector3(start2, offset, start1), new Vector3(end2, offset, end1));
        }
    }
    
    private void OnHoverOverGround(Vector3 hoverPoint)
    {
        var e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        var eventType = e.GetTypeForControl(controlID);

        switch (eventType)
        {
            case EventType.MouseDown:
                if (e.button != 2)
                {
                    leftPressed = e.button == 0;
                    GUIUtility.hotControl = controlID;

                    OnAction(hoverPoint, leftPressed);
                    e.Use();
                }
                break;
            case EventType.MouseUp:
                if (leftPressed)
                {
                    leftPressed = false;
                    GUIUtility.hotControl = 0;
                    
                    previousMouseEventLocation = Vector3.positiveInfinity;
                    
                    e.Use();
                }
                break;
            case EventType.MouseDrag:
                if(e.button != 2 && Vector3.Distance(hoverPoint, previousMouseEventLocation) > 0.5f)
                {
                    leftPressed = e.button == 0;
                    previousMouseEventLocation = hoverPoint;
                    OnAction(hoverPoint, leftPressed);
                    e.Use();
                }
                    break;
            case EventType.ScrollWheel:
                if(Event.current.modifiers != EventModifiers.Control) break;
                
                var scrollAmount = Event.current.delta.y;
                paintSize += scrollAmount / 8;
                paintSize = Mathf.Clamp(paintSize, 0.2f, 1000f);

                GUIUtility.hotControl = controlID;
                Event.current.Use();
            break;
        }
        
        Handles.color = Color.red;
        if (state == DrawState.ADDWATER)
        {
            Handles.DrawWireCube(hoverPoint + Vector3.up, new Vector3(1, 0, 1));
        }
        else
        {
            Handles.DrawWireDisc(hoverPoint + Vector3.up, Vector3.up, paintSize);
        }
    }

    private void OnAction(Vector3 hoverPoint, bool leftClick)
    {
        if (state == DrawState.ADDWATER)
        {
            if (leftClick)
            {
                var xy = WorldSpaceToXY(hoverPoint);
                OnMakeWaterTile(xy[0], xy[1]);
            }
        }
        else
        {
            var curLayer = state == DrawState.ADDTREES ? _terrainGen.treeLayer: _terrainGen.rockLayer;
            //Secondary Action Delete
            if (!leftClick)
            {
                var returnColliders = Physics.OverlapSphere(hoverPoint, paintSize, curLayer);
                for (int x = 0; x < returnColliders.Length; x++)
                    Undo.DestroyObjectImmediate(returnColliders[x].gameObject);
                
                return;
            }
            var correctArray = state == DrawState.ADDTREES ? _terrainGen.treeTiles : _terrainGen.rockTiles;
            var correctParent = state == DrawState.ADDTREES ? treeParent : rockParent;
            
            for (int x = 0; x < paintNumObjects; x++)
            {
                var randomComponent = Random.insideUnitCircle * paintSize;
                var instantiated = Instantiate(correctArray[Random.Range(0, correctArray.Length)], hoverPoint + new Vector3(randomComponent.x, 0, randomComponent.y), Quaternion.identity, correctParent.transform);
                Undo.RegisterCreatedObjectUndo(instantiated, "Created_Obj");
            }
        }
    }
}

public static class Extensions {
    public static T GetValue<T>(this SerializedProperty obj) where T: class
    {
        return obj.objectReferenceValue as T;
    }
    public static void AutoWeld (Mesh mesh, float threshold, float bucketStep) {
     Vector3[] oldVertices = mesh.vertices;
     Vector3[] newVertices = new Vector3[oldVertices.Length];
     int[] old2new = new int[oldVertices.Length];
     int newSize = 0;
 
     // Find AABB
     Vector3 min = new Vector3 (float.MaxValue, float.MaxValue, float.MaxValue);
     Vector3 max = new Vector3 (float.MinValue, float.MinValue, float.MinValue);
     for (int i = 0; i < oldVertices.Length; i++) {
       if (oldVertices[i].x < min.x) min.x = oldVertices[i].x;
       if (oldVertices[i].y < min.y) min.y = oldVertices[i].y;
       if (oldVertices[i].z < min.z) min.z = oldVertices[i].z;
       if (oldVertices[i].x > max.x) max.x = oldVertices[i].x;
       if (oldVertices[i].y > max.y) max.y = oldVertices[i].y;
       if (oldVertices[i].z > max.z) max.z = oldVertices[i].z;
     }
 
     // Make cubic buckets, each with dimensions "bucketStep"
     int bucketSizeX = Mathf.FloorToInt ((max.x - min.x) / bucketStep) + 1;
     int bucketSizeY = Mathf.FloorToInt ((max.y - min.y) / bucketStep) + 1;
     int bucketSizeZ = Mathf.FloorToInt ((max.z - min.z) / bucketStep) + 1;
     List<int>[,,] buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];
 
     // Make new vertices
     for (int i = 0; i < oldVertices.Length; i++) {
       // Determine which bucket it belongs to
       int x = Mathf.FloorToInt ((oldVertices[i].x - min.x) / bucketStep);
       int y = Mathf.FloorToInt ((oldVertices[i].y - min.y) / bucketStep);
       int z = Mathf.FloorToInt ((oldVertices[i].z - min.z) / bucketStep);
 
       // Check to see if it's already been added
       if (buckets[x, y, z] == null)
         buckets[x, y, z] = new List<int> (); // Make buckets lazily
 
       for (int j = 0; j < buckets[x, y, z].Count; j++) {
         Vector3 to = newVertices[buckets[x, y, z][j]] - oldVertices[i];
         if (Vector3.SqrMagnitude (to) < threshold) {
           old2new[i] = buckets[x, y, z][j];
           goto skip; // Skip to next old vertex if this one is already there
         }
       }
 
       // Add new vertex
       newVertices[newSize] = oldVertices[i];
       buckets[x, y, z].Add (newSize);
       old2new[i] = newSize;
       newSize++;
 
     skip:;
     }
 
     // Make new triangles
     int[] oldTris = mesh.triangles;
     int[] newTris = new int[oldTris.Length];
     for (int i = 0; i < oldTris.Length; i++) {
       newTris[i] = old2new[oldTris[i]];
     }
     
     Vector3[] finalVertices = new Vector3[newSize];
     for (int i = 0; i < newSize; i++)
       finalVertices[i] = newVertices[i];
 
     mesh.Clear();
     mesh.vertices = finalVertices;
     mesh.triangles = newTris;
     mesh.RecalculateNormals ();
     MeshUtility.Optimize(mesh);
    }

}
