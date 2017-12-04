using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor: Editor
{

    private bool leftPressed;
    private Vector3 previousMouseEventLocation = Vector3.positiveInfinity;
    private Spawner _spawner;
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        DrawPropertiesExcluding(serializedObject, new []{ "Assets"});
        EditorCustomGUIUtility.ShowList(serializedObject.FindProperty("Assets"));
        
        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        _spawner = target as Spawner;
        
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, _spawner.groundLayer))
        {
            DrawPaintArea(hit.point, _spawner.SpawnRange, _spawner.SpawnShape);
            OnHoverOverGround(hit.point);
        }
    }

    private void DrawPaintArea(Vector3 position, float range, Spawner.Shape spawnShape)
    {
        Color drawColor = new Color(0.41f, 0.76f, 1f, 0.6f);
        if (spawnShape == Spawner.Shape.Circle)
        {
            var previousColor = Handles.color;
            Handles.color = drawColor;
            Handles.DrawSolidDisc(position, Vector3.up, range);
            Handles.color = previousColor;
        }
        else
        {
            Handles.DrawSolidRectangleWithOutline(new []
            {
                new Vector3(position.x - range, position.y, position.z - range), 
                new Vector3(position.x - range, position.y, position.z + range), 
                new Vector3(position.x + range, position.y, position.z + range), 
                new Vector3(position.x + range, position.y, position.z - range)
            }, drawColor, drawColor);
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
                if (e.button != 2 && (e.modifiers & EventModifiers.Shift) == 0)
                {
                    leftPressed = e.button == 0;
                    GUIUtility.hotControl = controlID;
                    OnAction(hoverPoint, leftPressed);
                    e.Use();
                }
                break;
            case EventType.MouseUp:
                if (leftPressed && (e.modifiers & EventModifiers.Shift) == 0)
                {
                    leftPressed = false;
                    GUIUtility.hotControl = 0;
                    previousMouseEventLocation = Vector3.positiveInfinity;
                    e.Use();
                }
                break;
            case EventType.MouseDrag:
                if(e.button != 2 && Vector3.Distance(hoverPoint, previousMouseEventLocation) > 0.5f && (e.modifiers & EventModifiers.Shift) == 0)
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
                _spawner.SpawnRange += scrollAmount / 8;
                _spawner.SpawnRange = Mathf.Clamp(_spawner.SpawnRange, 0.2f, 1000f);

                GUIUtility.hotControl = controlID;
                Event.current.Use();
                break;
        }
    }

    private void OnAction(Vector3 hoverPoint, bool leftPressed)
    {
        //Secondary Action Delete
        if (!leftPressed)
        {
            var returnColliders = Physics.OverlapSphere(hoverPoint, _spawner.SpawnRange);
            for (int x = 0; x < returnColliders.Length; x++)
            {
                if (returnColliders[x].gameObject.GetRootParent().name == _spawner.ParentName)
                {
                    Undo.DestroyObjectImmediate(returnColliders[x].gameObject);
                }
            }
            return;
        }
        var parent = FindRoot();
        for (int x = 0; x < _spawner.Instances; x++)
        {
            Vector2 randomComponent;
            if (_spawner.SpawnShape == Spawner.Shape.Circle)
            {
                randomComponent = Random.insideUnitCircle * _spawner.SpawnRange;
            }
            else
            {
                randomComponent = new Vector2(Random.Range(-1f, 1f) * _spawner.SpawnRange, Random.Range(-1f, 1f) * _spawner.SpawnRange);
            }
            
            var asset = PickAsset(_spawner.Assets);
            var instantiated = Instantiate(asset, hoverPoint + new Vector3(randomComponent.x, 0, randomComponent.y), Quaternion.identity);
            
            
            var instantiatedModel = instantiated;
            if (instantiatedModel.GetComponent<Collider>() == null)
            {
                var instantiatedCollider = instantiatedModel.AddComponent<BoxCollider>();
                var instantiatedBounds = instantiatedModel.GetComponent<Renderer>().bounds;
                instantiatedCollider.center = instantiatedBounds.center - instantiated.transform.position;
                instantiatedCollider.size = instantiatedBounds.size;
            }

            instantiated.transform.SetParent(parent.transform, true);
            Undo.RegisterCreatedObjectUndo(instantiated, "Created_Obj");
        }
    }

    public GameObject FindRoot()
    {
        if (_spawner.ParentName == "")
        {
            Debug.LogError("Spawner parent name not set.");
            return null;
        }
        var parent = GameObject.Find(_spawner.ParentName) ?? new GameObject(name = _spawner.ParentName);

        return parent;
    }
    private GameObject PickAsset(List<SpawnerAsset> spawnerAssets)
    {
        var probability = Random.Range(0.0f, 1.0f);
        var totalSuccessRate = spawnerAssets.Sum(x => x.SuccessRate);
        
        var orderedAssets = spawnerAssets.OrderBy(x => x.SuccessRate).ToList();
        for (int x = 0; x < orderedAssets.Count; x++)
        {
            var normalizedSuccessRate = orderedAssets[x].SuccessRate / totalSuccessRate;
            if (normalizedSuccessRate > probability)
                return orderedAssets[x].Asset.obj;
        }

        return spawnerAssets[0].Asset.obj;
    }
}
