using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorCustomGUIUtility {
	
	public static void ShowList (SerializedProperty list) {
		if (!list.isArray)
		{
			EditorGUILayout.HelpBox(list.name + " is not a list or array.", MessageType.Error);
			return;
		}
		EditorGUILayout.PropertyField(list);
		EditorGUI.indentLevel += 1;
		if (list.isExpanded)
		{
			for (var i = 0; i < list.arraySize; i++)
			{
				if(i > 0)
					EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Element " + (i + 1), EditorStyles.boldLabel);
				GUILayout.FlexibleSpace();
				var deleteElement = GUILayout.Button("-", EditorStyles.miniButtonRight);
				if (deleteElement)
				{
					var initialSize = list.arraySize;
					list.DeleteArrayElementAtIndex(i);
					if(list.arraySize == initialSize)
						list.DeleteArrayElementAtIndex(i);
					
					continue;
				}
				
				EditorGUILayout.EndHorizontal();
				
				var element = list.GetArrayElementAtIndex(i).Copy();
				var currentDepth = element.depth;
				while(element.NextVisible(true) && element.depth > currentDepth)
				{
					EditorGUILayout.PropertyField(element, true);
				}
			}

			if (GUILayout.Button("+", EditorStyles.miniButton))
			{
				list.arraySize += 1;
			}
		}
		EditorGUI.indentLevel -= 1;
		
	}
	
	public static void DrawPreviewTexture(Rect position, Texture2D asset)
	{
		var style = new GUIStyle {normal = {background = asset}};
		EditorGUI.LabelField(position, GUIContent.none, style); 
	}
}
