#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Move2D
{
	public class HideWhenFalseAttribute : PropertyAttribute
	{
		public readonly string hideBoolean;

		public HideWhenFalseAttribute (string booleanName)
		{
			this.hideBoolean = booleanName;
		}
	}

	[CustomPropertyDrawer (typeof(HideWhenFalseAttribute))]
	public class HideWhenFalseDrawer : PropertyDrawer
	{

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			var hiddenAttribute = attribute as HideWhenFalseAttribute;
			var boolProperty = property.serializedObject.FindProperty (hiddenAttribute.hideBoolean);

			if (boolProperty.boolValue)
				EditorGUI.PropertyField (position, property, label, true);
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			var hiddenAttribute = attribute as HideWhenFalseAttribute;
			var boolProperty = property.serializedObject.FindProperty (hiddenAttribute.hideBoolean);

			if (!boolProperty.boolValue)
				return 0f;
		
			return EditorGUI.GetPropertyHeight (property);
		}
	}
}
#endif