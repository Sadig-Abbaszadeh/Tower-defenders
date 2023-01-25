using UnityEngine;

namespace DartsGames.CUT
{
    public class CreateObjectsAroundThis : MonoBehaviour
    {
        public GameObject cloneObject;
        [Range(2, 50)]
        public int childCount = 2;
        [Range(.1f, 100)]
        public float radius = .1f;
        public float offsetDegree = 0;
        public Vector3 rotation = Vector3.zero;
        [Range(.01f, 100)]
        public float scaleX = 1, scaleY = 1, scaleZ = 1;

        [HideInInspector]
        public Transform[] objects = new Transform[0];

        public void Generate()
        {
            Vector3 _rotation = new Vector3(mod(rotation.x, 360), mod(rotation.y, 360), mod(rotation.z, 360));

            if (childCount != objects.Length)
            {
                SpawnNewObjects(childCount, cloneObject, transform);
            }

            Rotate(_rotation, transform);
            ChangeScale(new Vector3(scaleX, scaleY, scaleZ), transform);
            Move(radius, transform, _rotation);
        }

        private void SpawnNewObjects(int count, GameObject original, Transform transform)
        {
            DestroyAll();

            objects = new Transform[count];

            float angle = 360f / count;

            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(original, transform.position, Quaternion.identity, transform);
                obj.name = "child" + i;

                objects[i] = obj.transform;
            }
        }

        private void Move(float radius, Transform transform, Vector3 rotation)
        {
            int count = objects.Length;
            float angle = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float tetha = mod((angle * i + mod(offsetDegree, 360)), 360);

                float x = radius * Mathf.Cos(tetha * Mathf.Deg2Rad);
                float z = Mathf.Sqrt(radius * radius - x * x);

                if (tetha > 180)
                    z *= -1;

                objects[i].position = new Vector3(x + transform.position.x, transform.position.y, z + transform.position.z);
            }
        }

        private void Rotate(Vector3 rotation, Transform transform)
        {
            int count = objects.Length;
            float angle = 360f / count;

            //
            for (int i = 0; i < count; i++)
            {
                objects[i].rotation = Quaternion.identity;
                objects[i].Rotate(rotation, Space.World);
                objects[i].RotateAround(transform.position, Vector3.up, mod((-angle * i - mod(offsetDegree, 360)), 360));
            }
        }

        private void ChangeScale(Vector3 scale, Transform transform)
        {
            foreach (Transform t in objects)
            {
                t.SetParent(null);
                t.localScale = scale;
                t.SetParent(transform);
            }
        }

        public void DestroyAll()
        {
            foreach (Transform t in objects)
            {
                DestroyImmediate(t.gameObject);
            }
        }

        private float mod(float left, float right) => (left % right + right) % right;
    }
}