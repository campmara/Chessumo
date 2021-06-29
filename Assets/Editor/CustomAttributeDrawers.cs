using UnityEngine;
using UnityEditor;

/// <summary>
/// Adds a [ReadOnly] tag that can be utilized to designate member variables as "Read Only", 
/// allowing them to be modifiable at runtime but not in the inspector. Values will appear in 
/// the inspector slightly greyed-out.
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute)attribute;

        // Use a specified name if there is one
        string displayName = (readOnlyAttribute.displayName != "") ? readOnlyAttribute.displayName : property.displayName;

        switch (property.propertyType) {
            case SerializedPropertyType.Float:
                EditorGUI.LabelField(position, displayName, property.floatValue.ToString());
                break;
            case SerializedPropertyType.Integer:
                EditorGUI.LabelField(position, displayName, property.intValue.ToString());
                break;
            case SerializedPropertyType.Vector3:
                EditorGUI.LabelField(position, displayName, property.vector3Value.ToString());
                break;
            default:
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
                break;
        }
    }
}
