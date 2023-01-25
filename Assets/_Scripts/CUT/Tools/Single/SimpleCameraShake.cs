using DartsGames.CUT.UnityExtensions;
using UnityEngine;

namespace DartsGames.CUT
{
    [DisallowMultipleComponent]
    public class SimpleCameraShake : MonoBehaviour
    {
        [SerializeField]
        protected float shakeAmount = .15f, shakeDuration = .18f;

        protected bool isShaking = false;

        protected Vector3 initialPos;

        public void ShakeCamera(float shakeAmount)
        {
            if (!isShaking)
            {
                initialPos = transform.position;

                var time = 0f;
                isShaking = true;

                this.DoWhile(() => time < shakeDuration,
                    () =>
                    {
                        time += Time.deltaTime;
                        transform.position = initialPos + Random.insideUnitSphere * shakeAmount;
                    }, () =>
                    {
                        isShaking = false;
                        transform.position = initialPos;
                    });
            }
        }

        public void ShakeCamera() => ShakeCamera(shakeAmount);
    }
}