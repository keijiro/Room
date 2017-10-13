using UnityEngine;
using UnityEditor;

namespace Room
{
    public class WallMaterialInspector : ShaderGUI
    {
        static class Labels
        {
            public static GUIContent normalMap = new GUIContent("Normal Map");
            public static GUIContent occlusionMap = new GUIContent("Occlusion Map");
        }

        public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
        {
            // Base parameters
            editor.ShaderProperty(FindProperty("_Color1", props), "Primary Color");
            editor.ShaderProperty(FindProperty("_Color2", props), "Secondary Color");
            editor.ShaderProperty(FindProperty("_Metallic", props), "Metallic");
            editor.ShaderProperty(FindProperty("_Smoothness", props), "Smoothness");

            EditorGUILayout.Space();

            // Base maps
            editor.TexturePropertySingleLine(
                Labels.normalMap, FindProperty("_NormalMap", props)
            );

            editor.TexturePropertySingleLine(
                Labels.occlusionMap,
                FindProperty("_OcclusionMap", props),
                FindProperty("_OcclusionMapStrength", props)
            );

            EditorGUILayout.Space();

            // Detail maps
            EditorGUILayout.LabelField("Details", EditorStyles.boldLabel);

            editor.TexturePropertySingleLine(
                Labels.normalMap,
                FindProperty("_DetailNormalMap", props),
                FindProperty("_DetailNormalMapScale", props)
            );

            editor.ShaderProperty(FindProperty("_DetailMapScale", props), "Scale");
        }
    }
}
