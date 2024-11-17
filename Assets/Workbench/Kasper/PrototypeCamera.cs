using JetBrains.Annotations;
using UnityEngine;

namespace KasperPrototype
{
    public class PrototypeCamera : MonoBehaviour
    {
        private static PrototypeCamera instance;

        public Transform followTransform;

        public float followSharpness = 5f;
        public float heightOffset = 25f;
        public float zOffset = -15f;
        public float xOffset = 30f;

        [Header("Intro animation")]
        public bool useIntroCinematic = false;
        public float introStartZOffset = 600;
        public float introStartXOffset = -300f;
        public float introSpeed = .25f;
        private bool fadeInComplete = false;

        private float currentZOffset = 0f;
        private float currentXOffset = 0f;

        private void Awake()
        {
            if (instance != null)
                Destroy(gameObject);

            instance = this;
        }

        private void Start()
        {
            if (useIntroCinematic)
            {
                currentZOffset = introStartZOffset;
                currentXOffset = introStartXOffset;
            }
            else
            {
                currentZOffset = zOffset;
                currentXOffset = xOffset;
            }
        }

        private void Update()
        {
            if (!useIntroCinematic)
                return;

            if (!fadeInComplete)
            {
                currentZOffset = Mathf.Lerp(currentZOffset, zOffset, 1f - Mathf.Exp(-introSpeed * Time.deltaTime));
                currentXOffset = Mathf.Lerp(currentXOffset, xOffset, 1f - Mathf.Exp(-introSpeed * Time.deltaTime));

                if (Mathf.Approximately(currentZOffset, zOffset))
                {
                    fadeInComplete = true;
                }
            }
        }

        private void FixedUpdate()
        {
            Vector3 followOffset = new Vector3(currentXOffset, heightOffset, currentZOffset);

            Vector3 targetPosition = followTransform.position + followOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));

            Vector3 directionToHelicopter = followTransform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(directionToHelicopter, Vector3.up);
        }

        public static void SetFollowTransform(Transform p_followTransform)
        {
            instance.followTransform = p_followTransform;
        }
    }
}