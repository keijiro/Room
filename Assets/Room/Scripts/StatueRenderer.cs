using UnityEngine;
using UnityEngine.Timeline;

namespace Room
{
    [ExecuteInEditMode]
    public class StatueRenderer : MonoBehaviour, ITimeControl
    {
        #region Editable attributes

        [SerializeField] Mesh _mesh;
        [SerializeField] Material _material;

        enum Mode { Default, Helix }
        [SerializeField] Mode _mode;

        #endregion

        #region Private variables and methods

        Material _tempMaterial;

        bool _underTimeControl;
        float _time;

        void SyncRenderMode()
        {
            var index = (int)_tempMaterial.GetFloat("_Mode");
            if (index == (int)_mode) return;

            var keyword = "_MODE_" + _mode.ToString().ToUpper();
            _tempMaterial.shaderKeywords = new string [] { keyword };
            _tempMaterial.SetFloat("_Mode", (float)_mode);
        }

        #endregion

        #region MonoBehaviour implementation

        void OnDestroy()
        {
            if (_tempMaterial != null)
            {
                if (Application.isPlaying)
                    Destroy(_tempMaterial);
                else
                    DestroyImmediate(_tempMaterial);
            }
        }

        void Update()
        {
            if (_material == null || _mesh == null) return;

            // Lazy initialization
            if (_tempMaterial == null)
            {
                _tempMaterial = new Material(_material);
                _tempMaterial.hideFlags = HideFlags.DontSave;
            }

            // Update the material properties.
            _tempMaterial.CopyPropertiesFromMaterial(_material);
            _tempMaterial.SetFloat("_LocalTime", _time);
            SyncRenderMode();

            // Draw request
            Graphics.DrawMesh(
                _mesh, transform.localToWorldMatrix,
                _tempMaterial, gameObject.layer
            );

            // Update the time.
            if (!_underTimeControl)
            {
                if (Application.isPlaying)
                    _time += Time.deltaTime;
                else
                    _time = 0;
            }
        }

        #endregion

        #region ITimeControl functions

        public void OnControlTimeStart()
        {
            _underTimeControl = true;
        }

        public void OnControlTimeStop()
        {
            _underTimeControl = false;
        }

        public void SetTime(double time)
        {
            _time = (float)time;
        }

        #endregion
    }
}
