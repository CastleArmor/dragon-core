using System;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Dragon.Core.Editor
{
    public class TopTitleAttributeDrawer : OdinAttributeDrawer<TopTitleAttribute>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //Rect rect = EditorGUILayout.GetControlRect();
//
            //if (label != null)
            //{
            //    EditorGUI.LabelField(rect, label);
            //}
//
            //for (int i = 0; i < this.Property.Children.Count; i++)
            //{
            //    this.Property.Children[i].Draw();
            //}

            string typeAddition = "";
            if (Attribute.ShowGenericName)
            {
                Type[] genericArgs = Property.Info.TypeOfValue.GetGenericArguments();
                if (!Attribute.ShowTypeOnSuffix)
                {
                    typeAddition += " [";
                }
                int index = 0;
                foreach (Type genType in genericArgs)
                {
                    if (genericArgs.Length-1 == index)
                    {
                        if (Attribute.LastIsReturn)
                        {
                        
                        }
                        else
                        {
                            typeAddition += genType.GetNiceName();
                        }
                    }
                    else
                    {
                        if (Attribute.LastIsReturn)
                        {
                            typeAddition += "↖";
                        }
                        typeAddition += genType.GetNiceName();
                    }
                    if (genericArgs.Length-1 != index)
                    {
                        typeAddition += Attribute.PerGenericArgString;
                    }
                    else
                    {
                        if (Attribute.LastIsReturn)
                        {
                            typeAddition += "☇" + genType.GetNiceName();
                        }
                    }

                    index += 1;
                }

                if (!Attribute.ShowTypeOnSuffix)
                {
                    typeAddition += "]";
                }
            }

            Rect titleRect = new Rect();
            GUIStyle nameStyle = Attribute.IsNameCentered? SirenixGUIStyles.LabelCentered : SirenixGUIStyles.Label;
            GUIStyle prefixStyle = SirenixGUIStyles.Label;
            GUIStyle suffixStyle = SirenixGUIStyles.TitleRight;
            if (label == null)
            {
                SirenixEditorGUI.BeginBox(label);
            }
            else
            {
                SirenixEditorGUI.BeginBox();
                SirenixEditorGUI.BeginBoxHeader();
                float fieldWidth = EditorGUIUtility.fieldWidth;
                EditorGUIUtility.fieldWidth = 10f;
                Rect controlRect = EditorGUILayout.GetControlRect(false);
                titleRect = controlRect;
                controlRect = controlRect.AddY(-1);
                EditorGUIUtility.fieldWidth = fieldWidth;
                prefixStyle.richText = true;
                nameStyle.richText = true;
                suffixStyle.richText = true;
                string nameLabel = label.text + typeAddition;
                if (Attribute.BoldName)
                {
                    nameLabel = "<b>" + nameLabel + "</b>";
                }

                if (Attribute.ShowNameOnPrefix)
                {
                    GUI.Label(controlRect, "<size=15>"+Attribute.NamePrefix+"</size>"+" <size=13><b>"+label.text+"</b></size>", prefixStyle);
                }
                else
                {
                    GUI.Label(controlRect, "<size=18>"+Attribute.NamePrefix+"</size>", prefixStyle);
                }
                if (Attribute.ShowTypeOnSuffix)
                {
                    typeAddition = typeAddition.Replace("DS_", "");
                    GUI.Label(controlRect, "<size=12>"+typeAddition+"</size> <size=15>"+Attribute.NameSuffix+"</size>", suffixStyle);
                }
                else
                {
                    GUI.Label(controlRect, "<size=18>"+Attribute.NameSuffix+"</size>", suffixStyle);
                }

                if (!Attribute.HideNameOnMid)
                {
                    GUI.Label(controlRect, nameLabel, nameStyle);
                }
                SirenixEditorGUI.EndBoxHeader();
            }

            InspectorProperty directParent = Property.Parent;

            if (Attribute.SetTransform || Attribute.SetParentObject || Attribute.SetName)
            {
                InspectorProperty firstUnityObjectProperty = null;
                UnityEngine.Object firstUnityObject = null;
                
                InspectorProperty loopingProperty = Property;
                int max = 1000;
                int count = 0;
                while (loopingProperty != null && firstUnityObjectProperty == null)
                {
                    if (loopingProperty.ValueEntry.WeakSmartValue is UnityEngine.Object foundUnityObject)
                    {
                        firstUnityObjectProperty = loopingProperty;
                        firstUnityObject = foundUnityObject;
                    }

                    loopingProperty = loopingProperty.Parent;
                    
                    count++;
                    if (count > max)
                    {
                        Debug.LogError("Parent finding loop count exceeded " + max + ", you might have a problem");
                        Debug.LogError("Last Looping Property = " + loopingProperty);
                        Debug.LogError("First Unity Object = " + firstUnityObject);
                        break;
                    }
                }
                
                if (firstUnityObjectProperty != null && directParent != null)
                {
                    if (firstUnityObject != null)
                    {
                        Type type = Property.ValueEntry.WeakSmartValue.GetType();
                        object self = Property.ValueEntry.WeakSmartValue;
                        Type containerType = firstUnityObject.GetType();
                        
                        //Handle unity engine object operations.
                        if (Attribute.SetName)
                        {
                            PropertyInfo namePropInfo = type.GetProperty("name");
                            if (namePropInfo != null)
                            {
                                if ((string) namePropInfo.GetValue(self) != Property.Name)
                                {
                                    Undo.RecordObject(firstUnityObject,"NameSet-"+type);
                                    namePropInfo.SetValue(self,Property.Name);
                                    InspectorProperty thisProperty = Property;
                                    thisProperty.ValueEntry.WeakSmartValue = self;
                                    EditorUtility.SetDirty(firstUnityObject);
                                }
                            }
                        }
                        if (Attribute.SetParentObject)
                        {
                            PropertyInfo parentObjectPropInfo = type.GetProperty("parentObject");
                            if (parentObjectPropInfo != null)
                            {
                                if ((UnityEngine.Object) parentObjectPropInfo.GetValue(self) != firstUnityObject)
                                {
                                    Undo.RecordObject(firstUnityObject,"ParentObjectSet-"+type);
                                    parentObjectPropInfo.SetValue(self, firstUnityObject);
                                    
                                    InspectorProperty thisProperty = Property;
                                    thisProperty.ValueEntry.WeakSmartValue = self;
                                    
                                    EditorUtility.SetDirty(firstUnityObject);
                                }
                            }
                        }
                        
                        MonoBehaviour mono = firstUnityObject as MonoBehaviour;
                        if (mono != null)
                        {
                            if (containerType.IsSubclassOf(typeof(MonoBehaviour)))
                            {
                                if (Attribute.SetTransform)
                                {
                                    object value = containerType.GetProperty("transform").GetValue(mono);
                                    Transform transformValue = value as Transform;
                                    PropertyInfo propInfo = type.GetProperty("transform");
                                    if (propInfo != null)
                                    {
                                        if ((Transform) propInfo.GetValue(self) != transformValue)
                                        {
                                            Undo.RecordObject(mono,"TransformChangedInChildProp-"+type);
                                            propInfo.SetValue(self,transformValue);
                                            InspectorProperty thisProperty = Property;
                                            thisProperty.ValueEntry.WeakSmartValue = self;
                                            EditorUtility.SetDirty(mono);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        
            for (int index = 0; index < this.Property.Children.Count; ++index)
            {
                InspectorProperty child = this.Property.Children[index];
                child.Draw(child.Label);
            }
            SirenixEditorGUI.EndBox();
        }
    }
}