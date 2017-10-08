// KinoTube - Composite video/old TV artifacts simulation
// https://github.com/keijiro/KinoTube

using UnityEngine;
using UnityEditor;

namespace Kino
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Tube))]
    public class TubeEditor : Editor
    {
        SerializedProperty _bleeding;
        SerializedProperty _scanline;

        void OnEnable()
        {
            _bleeding = serializedObject.FindProperty("_bleeding");
            _scanline = serializedObject.FindProperty("_scanline");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_bleeding);
            EditorGUILayout.PropertyField(_scanline);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
