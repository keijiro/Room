using UnityEngine;

namespace Room
{
    [ExecuteInEditMode]
    public class Wiper : MonoBehaviour
    {
        [SerializeField] float _speed = 1;

        [SerializeField, HideInInspector] Shader _shader;

        Material _material;

        void OnDestroy()
        {
            if (_material != null)
                if (Application.isPlaying)
                    Destroy(_material);
                else
                    DestroyImmediate(_material);
        }

        void OnRenderImage(RenderTexture source, RenderTexture dest)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            var time = Application.isPlaying ? Time.time * _speed : 0;
            var seed = Mathf.FloorToInt(time);
            time = Mathf.Min(1, (time - seed) * 1.1f);

            var flip = (seed & 1) != 0;

            _material.SetColor("_Color1", flip ? Color.white : Color.black);
            _material.SetColor("_Color2", flip ? Color.black : Color.white);
            _material.SetInt("_Seed", seed);
            _material.SetFloat("_LocalTime", time);

            Graphics.Blit(source, dest, _material, 0);
        }
    }
}
