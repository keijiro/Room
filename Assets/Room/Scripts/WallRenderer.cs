using UnityEngine;
using UnityEngine.Timeline;

namespace Room
{
    [ExecuteInEditMode]
    public class WallRenderer : MonoBehaviour, ITimeControl
    {
        #region Editable attributes

        [Space]
        [SerializeField] Mesh _mesh;
        [SerializeField] Material _material;

        [Space]
        [SerializeField] Color _primaryColor = Color.white;
        [SerializeField] Color _floorColor = Color.white;
        [SerializeField] Color _ceilingColor = Color.white;
        [SerializeField] Color _secondaryColor = Color.white;

        enum Mode { Default, Wave, Stripe, Scroll, Ripple, Light }
        [Space, SerializeField] Mode _mode;
        [SerializeField] Transform _effectOrigin;

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
        MaterialPropertyBlock _tempSheet;
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

            if (_tempSheet == null)
                _tempSheet = new MaterialPropertyBlock();

            // Update the material properties.
            _tempMaterial.CopyPropertiesFromMaterial(_material);

            _tempMaterial.SetColor("_Color1", _primaryColor);
            _tempMaterial.SetColor("_Color2", _secondaryColor);
            _tempMaterial.SetFloat("_LocalTime", _time * _speed);
            _tempMaterial.SetFloat("_Threshold", _threshold);

            _tempMaterial.SetVector("_Params", new Vector4(
                _param1, _param2, _param3, _param4
            ));

            _tempMaterial.SetMatrix("_WorldToEffect", _effectOrigin != null ?
                _effectOrigin.worldToLocalMatrix : Matrix4x4.identity
            );

            _tempKeywords[0] = "_MODE_" + _mode.ToString().ToUpper();
            _tempMaterial.shaderKeywords = _tempKeywords;

            // Draw request
            var mtx = transform.localToWorldMatrix;
            Graphics.DrawMesh(_mesh, mtx, _tempMaterial, gameObject.layer, null, 0);
            Graphics.DrawMesh(_mesh, mtx, _tempMaterial, gameObject.layer, null, 1);
            Graphics.DrawMesh(_mesh, mtx, _tempMaterial, gameObject.layer, null, 4);

            _tempSheet.SetColor("_Color1", _floorColor);
            Graphics.DrawMesh(_mesh, mtx, _tempMaterial, gameObject.layer, null, 3, _tempSheet);

            _tempSheet.SetColor("_Color1", _ceilingColor);
            Graphics.DrawMesh(_mesh, mtx, _tempMaterial, gameObject.layer, null, 2, _tempSheet);

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
