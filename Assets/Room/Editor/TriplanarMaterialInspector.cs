using UnityEngine;
using UnityEditor;

namespace Room
{
    public class TriplanarMaterialInspector : ShaderGUI
    {
        static class Labels
        {
            public static GUIContent normalMap = new GUIContent("Normal Map");
        }

        public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
        {
            EditorGUI.BeginChangeCheck();

            editor.ColorProperty(FindProperty("_Color", props), "Color");
            editor.RangeProperty(FindProperty("_Metallic", props), "Metallic");
            editor.RangeProperty(FindProperty("_Smoothness", props), "Smoothness");

            editor.TexturePropertySingleLine(
                Labels.normalMap,
                FindProperty("_NormalMap", props),
                FindProperty("_NormalMapScale", props)
            );

            editor.FloatProperty(FindProperty("_TextureScale", props), "Texture Scale");
        }
    }
}
