using UnityEngine;
using UnityEngine.Timeline;

namespace Room
{
    [ExecuteInEditMode]
    public class WallOption : MonoBehaviour, ITimeControl
    {
        [SerializeField] MeshRenderer [] _renderers;

        [Space]
        [SerializeField] Color _color1 = Color.white;
        [SerializeField] Color _color2 = Color.gray;

        [Space]
        [SerializeField] float _speed = 1;
        [SerializeField] float _threshold = 0.5f;

        [Space]
        [SerializeField] float _param1;
        [SerializeField] float _param2;

        MaterialPropertyBlock _sheet;

        bool _underTimeControl;
        float _time;

        void Update()
        {
            if (_sheet == null) _sheet = new MaterialPropertyBlock();

            _sheet.SetColor("_Color1", _color1);
            _sheet.SetColor("_Color2", _color2);
            _sheet.SetVector("_PrimaryAxis", transform.forward);
            _sheet.SetVector("_SecondaryAxis", transform.up);
            _sheet.SetFloat("_LocalTime", _time * _speed);
            _sheet.SetFloat("_Threshold", _threshold);
            _sheet.SetFloat("_Param1", _param1);
            _sheet.SetFloat("_Param2", _param2);

            foreach (var renderer in _renderers)
                renderer.SetPropertyBlock(_sheet);

            if (!_underTimeControl)
            {
                if (Application.isPlaying)
                    _time += Time.deltaTime;
                else
                    _time = 0;
            }
        }

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
