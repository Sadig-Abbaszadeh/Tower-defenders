using DartsGames.CUT.Attributes;
using UnityEngine;

namespace DartsGames.CUT
{
    /// <summary>
    /// Make the plane object match the camera plane
    /// </summary>
    [DisallowMultipleComponent]
    [ExtendEditor]
    public class CameraClipPlane : MonoBehaviour
    {
        [SerializeField, Tooltip("Distance from camera")]
        protected float depth;
        [SerializeField, Tooltip("To which degree the quad should fill the plane in width and height"), Range(0, 2)]
        private float widthFactor = 1, heightFactor = 1;

        protected Vector3 up, right;

        protected Vector3[] vert = new Vector3[4]
        {
            new Vector3(-.5f, -.5f),
            new Vector3(.5f, -.5f),
            new Vector3(-.5f, .5f),
            new Vector3(.5f, .5f)
        };

        protected virtual void Start()
        {
            MatchClipPlane();
            SetQuad();
        }

        [InspectorButton]
        public void MatchClipPlane()
        {
            var cam = Camera.main;

            transform.rotation = cam.transform.rotation;
            transform.position = cam.transform.position + cam.transform.forward * depth;

            float v = cam.fieldOfView,
                h = Camera.VerticalToHorizontalFieldOfView(v, cam.aspect);

            transform.localScale = new Vector3(2 * depth * Mathf.Tan(h / 2 * Mathf.Deg2Rad) * widthFactor,
                2 * depth * Mathf.Tan(v / 2 * Mathf.Deg2Rad) * heightFactor, 1);
        }

        protected void SetQuad()
        {
            for (int i = 0; i < 4; i++)
                vert[i] = transform.TransformPoint(vert[i]);

            right = (vert[1] - vert[0]);
            up = (vert[2] - vert[0]);
        }
    }
}