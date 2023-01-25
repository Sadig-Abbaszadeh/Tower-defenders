using System;
using UnityEngine;

namespace DartsGames.CUT
{
    public class Joystick : MonoBehaviour
    {
        [SerializeField]
        private Transform center;
        [SerializeField]
        private bool isStatic = false;
        [SerializeField]
        private float radius;

        private Vector3 offset, v;

        private float m, oneOverRadius, // idk because mul is faster than div i guess
            screenXFactor, screenYFactor;

        public int IsHeldDown { get; private set; }

        public event Action OnReleased = () => { }, OnPressed = () => { };

        public float HorizontalInput => center.localPosition.x * oneOverRadius;
        public float VerticalInput => center.localPosition.y * oneOverRadius;

        private void Start()
        {
            SetUpSizes();

            if (!isStatic)
            {
                gameObject.SetActive(false);

                OnPressed += () => gameObject.SetActive(true);

                OnReleased += () => gameObject.SetActive(false);
            }
        }

        private void SetUpSizes()
        {
            RectTransform parentCanvas = null;
            Transform t = transform.parent;

            while (t != null)
            {
                if (t is RectTransform rt)
                {
                    parentCanvas = rt;
                    break;
                }

                t = t.parent;
            }

            if (parentCanvas == null)
                throw new ArgumentNullException("parentCanvas", "No parent with canvas component found");

            oneOverRadius = 1 / radius;
            offset = new Vector3(-parentCanvas.rect.width / 2, -parentCanvas.rect.height / 2);

            var c = Camera.main;

            screenXFactor = parentCanvas.rect.width / c.pixelWidth;
            screenYFactor = parentCanvas.rect.height / c.pixelHeight;
        }

        private Vector3 PixelToCanvasPosition(Vector3 pixelPosition) => new Vector3(pixelPosition.x * screenXFactor,
            pixelPosition.y * screenYFactor) + offset;

        public void SetPosition(Vector3 pixelPosition)
        {
            v = PixelToCanvasPosition(pixelPosition) - transform.localPosition;
            m = v.magnitude;

            center.localPosition = (m <= radius ? v : v / m * radius) * IsHeldDown;
        }

        public void Activate(Vector3 pixelPosition)
        {
            if (!isStatic)
                transform.localPosition = PixelToCanvasPosition(pixelPosition);

            IsHeldDown = 1;

            OnPressed();
        }

        public void Deactivate()
        {
            center.localPosition = Vector3.zero;

            IsHeldDown = 0;
            OnReleased();
        }
    }
}