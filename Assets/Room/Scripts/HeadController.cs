using UnityEngine;
using UnityEngine.Timeline;
using Klak.Math;

namespace Room
{
    [ExecuteInEditMode]
    public class HeadController : MonoBehaviour, ITimeControl
    {
        #region Editable attributes

        [SerializeField] Vector3 _positionNoise;
        [SerializeField] Vector3 _rotationNoise;
        [SerializeField] float _noiseSpeed;
        [SerializeField] float _gradientSpeed;
        [SerializeField] float _accent;
        [SerializeField] int _randomSeed;

        #endregion

        #region MonoBehaviour implementation

        MaterialPropertyBlock _shaderSheet;
        bool _underTimeControl;
        float _time;

        void Update()
        {
            if (_shaderSheet == null)
                _shaderSheet = new MaterialPropertyBlock();

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

            var trs = Matrix4x4.TRS(
                Vector3.Scale(pos, _positionNoise),
                Quaternion.Euler(Vector3.Scale(rot, _rotationNoise)),
                Vector3.one
            );

            var offs = _gradientSpeed * _time + _randomSeed;

            _shaderSheet.SetMatrix("_ExtraTransform", trs);
            _shaderSheet.SetFloat("_GradOffs", offs);
            GetComponent<MeshRenderer>().SetPropertyBlock(_shaderSheet);

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
