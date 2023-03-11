using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DataField<>))]
public class DataFieldDrawer : PropertyDrawer
{
    private bool _keyFieldToggled;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw transform and parentObject properties
        var transformProperty = property.FindPropertyRelative("_transform");
        var parentObjectProperty = property.FindPropertyRelative("_parentObject");
        EditorGUI.PropertyField(position, transformProperty);
        position.y += EditorGUI.GetPropertyHeight(transformProperty);
        EditorGUI.PropertyField(position, parentObjectProperty);
        position.y += EditorGUI.GetPropertyHeight(parentObjectProperty);

        // Draw key property
        var keyProperty = property.FindPropertyRelative("Key");
        if (!_keyFieldToggled && keyProperty.objectReferenceValue == null)
        {
            EditorGUI.HelpBox(position, "Please assign a DataKey or click 'Single' to create a new one.", MessageType.Info);
            position.y += EditorGUIUtility.singleLineHeight;

            position.width = 80f;
            if (GUI.Button(position, "Single"))
            {
                _keyFieldToggled = true;
            }

            position.width = EditorGUIUtility.currentViewWidth - position.x - 10f;
            position.x += 90f;
        }
        else
        {
            _keyFieldToggled = keyProperty.objectReferenceValue == null;
        }

        if (_keyFieldToggled || keyProperty.objectReferenceValue != null)
        {
            EditorGUI.PropertyField(position, keyProperty);
            position.y += EditorGUI.GetPropertyHeight(keyProperty);

            // Draw dataAddress, contextAddress, and relativeAddress properties
            var dataAddressProperty = property.FindPropertyRelative("DataAddress");
            var contextAddressProperty = property.FindPropertyRelative("ContextAddress");
            var relativeAddressProperty = property.FindPropertyRelative("RelativeAddress");
            EditorGUI.PropertyField(position, dataAddressProperty);
            position.y += EditorGUI.GetPropertyHeight(dataAddressProperty);

            if (dataAddressProperty.enumValueIndex != (int)DataAddress.Context)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(position, contextAddressProperty);
                position.y += EditorGUI.GetPropertyHeight(contextAddressProperty);

                if (contextAddressProperty.enumValueIndex == (int)ContextAddress.Relative)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.PropertyField(position, relativeAddressProperty);
                    position.y += EditorGUI.GetPropertyHeight(relativeAddressProperty);

                    if (relativeAddressProperty.enumValueIndex != (int)RelativeAddress.Self)
                    {
                        var relativeStackProperty = property.FindPropertyRelative("RelativeStack");
                        EditorGUI.PropertyField(position, relativeStackProperty);
                        position.y += EditorGUI.GetPropertyHeight(relativeStackProperty);
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            // Draw data property
            var dataProperty = property.FindPropertyRelative("_data");
            if (dataProperty != null)
            {
                EditorGUI.PropertyField(position, dataProperty);
                position.y += EditorGUI.GetPropertyHeight(dataProperty);
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_transform"));
        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_parentObject"));

        var keyProperty = property.FindPropertyRelative("Key");
        if (!_keyFieldToggled && keyProperty.objectReferenceValue == null)
        {
            height += EditorGUIUtility.singleLineHeight;
        }
        else
        {
            height += EditorGUI.GetPropertyHeight(keyProperty);

            var dataAddressProperty = property.FindPropertyRelative("DataAddress");
            if (dataAddressProperty.enumValueIndex != (int)DataAddress.Context)
            {
                height += EditorGUI.GetPropertyHeight(dataAddressProperty);

                var contextAddressProperty = property.FindPropertyRelative("ContextAddress");
                height += EditorGUI.GetPropertyHeight(contextAddressProperty);

                if (contextAddressProperty.enumValueIndex == (int)ContextAddress.Relative)
                {
                    var relativeAddressProperty = property.FindPropertyRelative("RelativeAddress");
                    height += EditorGUI.GetPropertyHeight(relativeAddressProperty);

                    if (relativeAddressProperty.enumValueIndex != (int)RelativeAddress.Self)
                    {
                        var relativeStackProperty = property.FindPropertyRelative("RelativeStack");
                        height += EditorGUI.GetPropertyHeight(relativeStackProperty);
                    }
                }

                if (dataAddressProperty.enumValueIndex == (int)DataAddress.GroupFirstMember)
                {
                    var groupKeyProperty = property.FindPropertyRelative("GroupKey");
                    height += EditorGUI.GetPropertyHeight(groupKeyProperty);
                }
            }

            var dataProperty = property.FindPropertyRelative("_data");
            if (dataProperty != null)
            {
                height += EditorGUI.GetPropertyHeight(dataProperty);
            }
        }

        return height + 10f;
    }
}