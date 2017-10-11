using UnityEngine;
using UnityEngine.Timeline;

namespace Room
{
    [ExecuteInEditMode]
    class CoreRenderer : MonoBehaviour, ITimeControl
    {
        #region Editable attributes

        [Space]
        [SerializeField, ColorUsage(false, true, 0, 8, 0.125f, 3)]
        Color _color = Color.white;
        [SerializeField] float _radius = 1;
        [Space]
        [SerializeField] float _noiseAmplitude;
        [SerializeField] float _noiseFrequency = 1;
        [SerializeField] float _noiseSpeed = 1;
        [Space]
        [SerializeField] float _flickering;
        [SerializeField] float _flickerSpeed = 50;

        #endregion

        #region Private objects

        [SerializeField, HideInInspector] Mesh _mesh;
        [SerializeField, HideInInspector] Shader _shader;

        Material _material;

        bool _underTimeControl;
        float _time;

        #endregion

        #region MonoBehaviour implementation

        void OnDestroy()
        {
            if (_material != null)
                if (Application.isPlaying)
                    Destroy(_material);
                else
                    DestroyImmediate(_material);
        }

        void Update()
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            var time = _noiseSpeed * _noiseFrequency * _time;
            var scale = 1.0f + Mathf.Sin(_time * _flickerSpeed) * _flickering;

            _material.SetColor("_Color", _color);
            _material.SetFloat("_Radius", _radius * scale);
            _material.SetFloat("_NoiseAmp", _noiseAmplitude);
            _material.SetFloat("_NoiseFreq", _noiseFrequency);
            _material.SetVector("_NoiseOffs", Vector3.forward * time);

            Graphics.DrawMesh(
                _mesh, transform.localToWorldMatrix,
                _material, gameObject.layer
            );

            if (!_underTimeControl)
            {
                if (Application.isPlaying)
                    _time += Time.deltaTime;
                else
                    _time = 0;
            }
        }

        #endregion

        #region ITimeControl implementation

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
