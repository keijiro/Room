using UnityEngine;
using UnityEngine.Timeline;
using Klak.Math;

namespace Room
{
    [ExecuteInEditMode]
    public class HeadRenderer : MonoBehaviour, ITimeControl
    {
        #region Editable attributes

        [Space]
        [SerializeField] float _scale = 1;
        [Space]
        [SerializeField] Vector3 _positionNoise;
        [SerializeField] Vector3 _rotationNoise;
        [SerializeField] float _noiseSpeed;
        [Space]
        [SerializeField] float _gradientFrequency;
        [SerializeField] float _gradientSpeed;
        [Space]
        [SerializeField] float _accent;
        [SerializeField] float _accentToScale;
        [Space]
        [SerializeField] Texture[] _faceTextures = new Texture[1];
        [SerializeField] float _faceSelect;
        [Space]
        [SerializeField] int _randomSeed;

        #endregion

        #region Private objects

        [SerializeField, HideInInspector] Mesh _mesh;
        [SerializeField, HideInInspector] Shader _shader;

        Material _faceMaterial;
        Material _headMaterial;

        bool _underTimeControl;
        float _time;

        #endregion

        #region MonoBehaviour implementation

        void OnDestroy()
        {
            if (_faceMaterial != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_faceMaterial);
                    Destroy(_headMaterial);
                }
                else
                {
                    DestroyImmediate(_faceMaterial);
                    DestroyImmediate(_headMaterial);
                }
            }
        }

        void Update()
        {
            // Lazy initialization
            if (_faceMaterial == null)
            {
                _faceMaterial = new Material(_shader);
                _headMaterial = new Material(_shader);
                _faceMaterial.hideFlags = HideFlags.DontSave;
                _headMaterial.hideFlags = HideFlags.DontSave;
            }

            // Animation
            var hash = new XXHash(_randomSeed);
            var time = _time * _noiseSpeed;
            const int octaves = 2;

            var pos = new Vector3(
                Perlin.Fbm(hash.Range(-1e3f, 1e3f, 0) + time, octaves),
                Perlin.Fbm(hash.Range(-1e3f, 1e3f, 1) + time, octaves),
                Perlin.Fbm(hash.Range(-1e3f, 1e3f, 2) + time, octaves)
            );

            var rot = new Vector3(
                Perlin.Fbm(hash.Range(-1e3f, 1e3f, 3) + time, octaves),
                Perlin.Fbm(hash.Range(-1e3f, 1e3f, 4) + time, octaves),
                Perlin.Fbm(hash.Range(-1e3f, 1e3f, 5) + time, octaves)
            );

            var trs = transform.localToWorldMatrix * Matrix4x4.TRS(
                Vector3.Scale(pos, _positionNoise),
                Quaternion.Euler(Vector3.Scale(rot, _rotationNoise)),
                Vector3.one * _scale * (1 + _accentToScale * _accent)
            );

            var offs = _gradientSpeed * _time + _randomSeed;

            // Update the material properties.
            var face = Mathf.FloorToInt(_faceSelect * _faceTextures.Length);
            _faceMaterial.SetTexture("_MainTex", _faceTextures[face]);

            _faceMaterial.SetColor("_Color", new Color(1, 1, 1, 1));
            _headMaterial.SetColor("_Color", new Color(1, 1, 1, 0));

            _headMaterial.SetFloat("_GradFreq", _gradientFrequency);
            _headMaterial.SetFloat("_GradOffs", offs);

            // Draw requests
            Graphics.DrawMesh(
                _mesh, trs, _headMaterial, gameObject.layer, null, 0
            );

            Graphics.DrawMesh(
                _mesh, trs, _faceMaterial, gameObject.layer, null, 1
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
