using UnityEngine;

namespace Room
{
    [ExecuteInEditMode]
    public class ObjectRenderer : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] Mesh _mesh;
        [SerializeField] Material _boxMaterial;
        [SerializeField] Material _coneMaterial;
        [SerializeField] Material _pillarMaterial;
        [SerializeField] Color _color;

        #endregion

        #region Private variables

        MaterialPropertyBlock _tempSheet;

        #endregion

        #region MonoBehaviour implementation

        void Update()
        {
            if (_mesh == null) return;

            // Lazy initialization
            if (_tempSheet == null)
                _tempSheet = new MaterialPropertyBlock();

            // Update the material properties.
            _tempSheet.SetColor("_Color1", _color);

            // Draw request
            var mtx = transform.localToWorldMatrix;

            if (_boxMaterial != null)
                Graphics.DrawMesh(_mesh, mtx, _boxMaterial, gameObject.layer, null, 0, _tempSheet);

            if (_coneMaterial != null)
                Graphics.DrawMesh(_mesh, mtx, _coneMaterial, gameObject.layer, null, 1, _tempSheet);

            if (_pillarMaterial != null)
                Graphics.DrawMesh(_mesh, mtx, _pillarMaterial, gameObject.layer, null, 2, _tempSheet);
        }

        #endregion
    }
}
