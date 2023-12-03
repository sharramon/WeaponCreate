using UnityEngine;
using UnityEditor;

namespace FullMetal
{
    [CustomEditor(typeof(HandPoseList))]
    public class HandPoseListEditor : Editor
    {
        SerializedProperty handPosesList;

        private void OnEnable()
        {
            // Setup the SerializedProperty
            handPosesList = serializedObject.FindProperty("_handPoses");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display and interact with the SerializedProperty
            ShowHandPosesList(handPosesList);

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowHandPosesList(SerializedProperty list)
        {
            EditorGUILayout.PropertyField(list);
            EditorGUI.indentLevel += 1;

            if (list.isExpanded)
            {
                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty handPosesProperty = list.GetArrayElementAtIndex(i);
                    SerializedProperty handChoiceProperty = handPosesProperty.FindPropertyRelative("_handChoice");

                    EditorGUILayout.PropertyField(handPosesProperty.FindPropertyRelative("_poseName"));
                    EditorGUILayout.PropertyField(handChoiceProperty);

                    HandChoice handChoice = (HandChoice)handChoiceProperty.enumValueIndex;
                    switch (handChoice)
                    {
                        case HandChoice.MainOnly:
                            EditorGUILayout.PropertyField(handPosesProperty.FindPropertyRelative("_mainHand"), true);
                            break;
                        case HandChoice.OffOnly:
                            EditorGUILayout.PropertyField(handPosesProperty.FindPropertyRelative("_offHand"), true);
                            break;
                        case HandChoice.BothHands:
                            EditorGUILayout.PropertyField(handPosesProperty.FindPropertyRelative("_mainHand"), true);
                            EditorGUILayout.PropertyField(handPosesProperty.FindPropertyRelative("_offHand"), true);
                            break;
                    }

                    if (GUILayout.Button("Remove Hand Poses " + (i + 1)))
                    {
                        list.DeleteArrayElementAtIndex(i);
                        break;
                    }
                }
            }

            EditorGUI.indentLevel -= 1;
        }
    }
}
