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

        enum Mode { Default, Vacs, Slice, Helix }
        [SerializeField] Mode _mode;

        [Space]
        [SerializeField] float _speed = 1;
        [SerializeField] float _threshold = 0.5f;
        [SerializeField] float _param1;
        [SerializeField] float _param2;
        [SerializeField] float _param3;
        [SerializeField] float _param4;

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
            _tempMaterial.SetFloat("_LocalTime", _time * _speed);
            _tempMaterial.SetFloat("_Threshold", _threshold);

            _tempMaterial.SetVector("_Params", new Vector4(
                _param1, _param2, _param3, _param4
            ));

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
