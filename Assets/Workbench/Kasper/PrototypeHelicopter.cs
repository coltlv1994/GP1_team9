using Grid;
using System.Collections.Generic;
using UnityEngine;

namespace KasperPrototype
{
    public class PrototypeHelicopter : MonoBehaviour
    {
        [SerializeField] private Rigidbody body;
        [SerializeField] new private Camera camera;

        [SerializeField] private float m_moveSpeed = 20f;
        [SerializeField] private float m_moveSharpness = 10f;
        [SerializeField] private float m_rotateSharpness = 7.5f;
        [SerializeField] private float m_lookInputLerpSpeed = 5f;

        [SerializeField] private float m_waterReleaseInterval = 1f;
        [SerializeField] private float m_waterReleaseRadius = 4f;
        private float m_WaterReleaseTimer = 0f;

        [SerializeField] private Vector2 m_moveInput;
        [SerializeField] private Vector2 m_lookInput;
        [SerializeField] private bool m_releaseWaterInput;
        [SerializeField] private bool m_gatherWaterInput;

        public GameObject m_CrashedHelicopterPrefab;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                gameObject.SetActive(false);

                if (Instantiate(m_CrashedHelicopterPrefab, transform.position, transform.rotation).TryGetComponent(out CrashedHelicopter crashedHelicopter))
                {
                    crashedHelicopter.InitializeCrash(body.linearVelocity);
                }
            }

            SetInput();
            if (m_releaseWaterInput)
            {
                m_WaterReleaseTimer += Time.deltaTime;

                if (m_WaterReleaseTimer >= m_waterReleaseInterval)
                {
                    m_WaterReleaseTimer = 0f;
                    ReleaseWater();
                }
            }
        }

        private void FixedUpdate()
        {
            MoveHelicopter();
            RotateHelicopter();
        }

        private void SetInput()
        {
            m_moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (m_moveInput != Vector2.zero)
            {
                m_lookInput = Vector2.Lerp(m_lookInput, m_moveInput, 1f - Mathf.Exp(-m_lookInputLerpSpeed * Time.deltaTime));
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_WaterReleaseTimer = 0f;
                m_releaseWaterInput = true;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
                m_releaseWaterInput = false;

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                m_gatherWaterInput = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
                m_gatherWaterInput = false;
        }

        private void MoveHelicopter()
        {
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(camera.transform.rotation * Vector3.forward, Vector3.up).normalized;

            if (cameraPlanarDirection.sqrMagnitude == 0f)
                cameraPlanarDirection = Vector3.ProjectOnPlane(camera.transform.rotation * Vector3.up, Vector3.up).normalized;

            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Vector3.up);
            Vector3 lookDirection = new Vector3(m_lookInput.x, 0f, m_lookInput.y);
            Vector3 lookDirectionVector = cameraPlanarRotation * lookDirection;

            Vector3 moveInputVector = cameraPlanarRotation * new Vector3(m_moveInput.x, 0f, m_moveInput.y);

            Vector3 targetVelocity = moveInputVector;

            targetVelocity = Vector3.ClampMagnitude(targetVelocity, 1f);
            targetVelocity = targetVelocity * m_moveSpeed;
            body.linearVelocity = Vector3.Lerp(body.linearVelocity, targetVelocity, 1f - Mathf.Exp(-m_moveSharpness * Time.deltaTime));
        }

        private void RotateHelicopter()
        {
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(camera.transform.rotation * Vector3.forward, Vector3.up).normalized;

            if (cameraPlanarDirection.sqrMagnitude == 0f)
                cameraPlanarDirection = Vector3.ProjectOnPlane(camera.transform.rotation * Vector3.up, Vector3.up).normalized;

            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Vector3.up);
            Vector3 lookDirection = new Vector3(m_lookInput.x, 0f, m_lookInput.y);
            Vector3 lookDirectionVector = cameraPlanarRotation * lookDirection;

            Quaternion targetRotation = transform.rotation;
            if (lookDirectionVector.sqrMagnitude > 0f)
            {
                targetRotation = Quaternion.LookRotation(lookDirectionVector, Vector3.up);
            }

            body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 1f - Mathf.Exp(-m_rotateSharpness * Time.deltaTime));
        }

        private void ReleaseWater()
        {
            //List<int> gridTiles = GridManager.GetInstance().TryGetTileInRange(transform.position, m_waterReleaseRadius);
            //GridManager.GetInstance().OnWaterComing(gridTiles);
            Debug.Log("Tick release water");
        }
    }
}