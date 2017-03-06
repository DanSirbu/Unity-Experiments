using UnityEngine;
using System.Collections;

public class ResetEditor : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR
        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
#endif
    }

}