using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(GameObjectVisual))]
public class GameObjectVisualDrawer : PropertyDrawer
{
	public static float DEFAULT_FIELD_HEIGHT = 16f;
	public static int TEXTURE_SIZE_MULT = 5;
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		position = position.SetHeight(DEFAULT_FIELD_HEIGHT);
		var gameObject = property.FindPropertyRelative("obj");
		EditorGUI.PropertyField(position, gameObject, label);

		int indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;


		
		var assetPreviewRect = position.SetX(position.x + EditorGUIUtility.labelWidth)
									   .SetY(position.y + DEFAULT_FIELD_HEIGHT)
									   .SetHeight(5*DEFAULT_FIELD_HEIGHT)
									   .SetWidth(5*DEFAULT_FIELD_HEIGHT);

		var gameObjectId = gameObject.objectReferenceInstanceIDValue;
		if (gameObjectId > 0)//Exists
		{
			Texture2D assetPreview = AssetPreview.GetAssetPreview(gameObject.objectReferenceValue);
			if (!AssetPreview.IsLoadingAssetPreview(gameObjectId) && assetPreview != null)
			{
				EditorCustomGUIUtility.DrawPreviewTexture(assetPreviewRect, assetPreview);
			}
		}

		EditorGUI.indentLevel = indentLevel;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return (TEXTURE_SIZE_MULT + 1) * DEFAULT_FIELD_HEIGHT;
	}
}