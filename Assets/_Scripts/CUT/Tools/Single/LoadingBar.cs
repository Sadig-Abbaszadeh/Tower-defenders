using UnityEngine;

namespace DartsGames.CUT
{
    [RequireComponent(typeof(RectTransform)), ExecuteAlways]
    public class LoadingBar : MonoBehaviour
    {
        private float maxWidth;

        private RectTransform rt;

        [SerializeField, Range(0, 1)]
        private float _value = 1;

        private void Start()
        {
            rt = GetComponent<RectTransform>();
            maxWidth = rt.rect.width;
            Value = _value;
        }

        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = Mathf.Clamp01(value);

#if UNITY_EDITOR
                if (rt == null)
                    return;
#endif
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth * _value);
            }
        }

        private void OnValidate()
        {
            Value = _value;
        }
    }
}