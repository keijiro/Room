using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Room
{
    public static class SplitMeshEditor
    {
        static Object[] SelectedMeshes {
            get { return Selection.GetFiltered(typeof(Mesh), SelectionMode.Deep); }
        }

        [MenuItem("Assets/Room/Split Mesh", true)]
        static bool ValidateConvertMesh()
        {
            return SelectedMeshes.Length > 0;
        }

        [MenuItem("Assets/Room/Split Mesh")]
        static void ConvertMesh()
        {
            var assets = new List<Object>();

            foreach (Mesh mesh in SelectedMeshes)
            {
                // Destination file path.
                var dirPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(mesh));
                var filename = (string.IsNullOrEmpty(mesh.name) ? "Split" : mesh.name + " Split") + ".asset";
                var assetPath = AssetDatabase.GenerateUniqueAssetPath(dirPath + "/" + filename);

                // Convert the mesh and store it as a new asset.
                assets.Add(ConvertMesh(mesh));
                AssetDatabase.CreateAsset(assets.Last(), assetPath);
            }

            // Save the generated assets.
            AssetDatabase.SaveAssets();

            // Select the generated assets.
            EditorUtility.FocusProjectWindow();
            Selection.objects = assets.ToArray();
        }

        static Mesh ConvertMesh(Mesh source)
        {
            var src_idx = source.GetIndices(0);
            var src_vtx = source.vertices;
            var src_nrm = source.normals;
            var src_tan = source.tangents;
            var src_uv0 = source.uv;

            var vcount = src_idx.Length;
            var vrefs1 = new List<Vector3>(vcount);
            var vrefs2 = new List<Vector3>(vcount);

            for (var i = 0; i < vcount; i += 3)
            {
                var v1 = src_vtx[src_idx[i    ]];
                var v2 = src_vtx[src_idx[i + 1]];
                var v3 = src_vtx[src_idx[i + 2]];

                vrefs1.Add(v2); vrefs2.Add(v3);
                vrefs1.Add(v3); vrefs2.Add(v1);
                vrefs1.Add(v1); vrefs2.Add(v2);
            }

            var mesh = new Mesh();
            mesh.name = "Lucy Converted";

            mesh.SetVertices(src_idx.Select(i => src_vtx[i]).ToList());
            mesh.SetNormals (src_idx.Select(i => src_nrm[i]).ToList());
            mesh.SetTangents(src_idx.Select(i => src_tan[i]).ToList());
            mesh.SetUVs  (0, src_idx.Select(i => src_uv0[i]).ToList());

            mesh.SetUVs(1, vrefs1);
            mesh.SetUVs(2, vrefs2);

            mesh.SetIndices(
                Enumerable.Range(0, vcount).ToArray(),
                MeshTopology.Triangles, 0
            );

            mesh.RecalculateBounds();
            mesh.UploadMeshData(true);

            return mesh;
        }
    }
}
