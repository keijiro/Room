using UnityEngine;
using UnityEngine.Timeline;

namespace Room
{
    [ExecuteInEditMode]
    public class StatueRenderer : MonoBehaviour, ITimeControl
    {
        #region Editable attributes

        [Space]
        [SerializeField] Mesh _mesh;
        [SerializeField] Material _material;

        enum Mode { Default, Slice, Helix }
        [Space, SerializeField] Mode _mode;

        [Space]
        [SerializeField] float _speed = 1;
        [SerializeField] float _threshold = 0.5f;

        [Space]
        [SerializeField] float _param1;
        [SerializeField] float _param2;
        [SerializeField] float _param3;
        [SerializeField] float _param4;

        #endregion

        #region Private variables and methods

        Material _tempMaterial;
        string [] _tempKeywords = new string [] { "" };

        bool _underTimeControl;
        float _time;

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

            _tempKeywords[0] = "_MODE_" + _mode.ToString().ToUpper();
            _tempMaterial.shaderKeywords = _tempKeywords;

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
