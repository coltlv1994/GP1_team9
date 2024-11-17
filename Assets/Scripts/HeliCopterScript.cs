using Grid;
using KasperPrototype;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeliCopterScript : MonoBehaviour
{
    [SerializeField] private Transform m_rotorTransform;
    [SerializeField] private Transform m_tailRotorTransform;
    [SerializeField] private float m_minRotorRotationSpeed = 500f, m_maxRotorRotationSpeed = 1000f;
    private float m_rotorRotation = 0;
    private float m_rotorSpeed = 500f;

    [SerializeField] private RotorTipTrailController m_rotorTipTrailController;
    [SerializeField] private RotorPitchController m_rotorPitchController;
    [SerializeField] private float m_linearVelocityMagnitude;
    [SerializeField] private float m_linearVelocityMinPitchThreshold = 10f;

    [SerializeField] private Transform m_spawnPoint;
    private bool m_enableFlying = true;
    private bool m_crashed;
    public float m_waterPickupSpeed;
    public float m_waterDropSpeed;
    public float m_waterReach;
    public float m_fallTimer;

    public GridManager m_gridManager = null;
    [SerializeField] private PlayerValues m_playerValues;
    private CrashedHelicopter m_crashedHelicopter;

    [SerializeField] private Rigidbody m_rigidBody;
    public GameObject m_helicopterOBJ;
    [SerializeField] new private Camera m_camera;

    [SerializeField] private float m_moveSpeed = 20f;
    [SerializeField] private float m_sprintSpeed = 35f;
    [SerializeField] private float m_moveSharpness = 10f;
    [SerializeField] private float m_sprintSharpness = .5f;
    [SerializeField] private float m_rotateSharpness = 7.5f;
    [SerializeField] private float m_lookInputLerpSpeed = 5f;

    [SerializeField] private Vector2 m_moveInput;
    [SerializeField] private Vector2 m_lookInput;

    private List<Vector2> m_positionHistory = new List<Vector2>();
    [SerializeField] private float m_positionHistoryUpdateDelay;
    [SerializeField] private int m_maxHistoryEntries;
    public GameObject m_crashedHelicopterPrefab;
    public MeshRenderer[] m_meshrenders;

    [SerializeField] private Bucket m_bucket;
    public float m_rotationSpeed;
    public GameObject m_pivotObject;
    private bool m_disableFlying;
    public float m_outOfBoundsTimer;

    private bool hasImpactPlayed;

    private void OnDisable()
    {
        SoundManager.StopRotorLoop();
        SoundManager.StopFireSource();
        SoundManager.StopWaterLoop();
    }

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_crashedHelicopter = GetComponent<CrashedHelicopter>();
        m_gridManager = GridManager.GetInstance();
        m_camera = Camera.main;
        StartCoroutine(UpdatePositionHistory());
        m_meshrenders = GetComponentsInChildren<MeshRenderer>();
        SoundManager.PlayRotorLoop();
        SoundManager.PlayFireLoop();
        SoundManager.PlayWaterLoop();
    }

    public void UpdateHeli()
    {

        if (m_attackDown && !m_crashed)
        {
            Tile m_myCurrentTile = m_gridManager.TryGetTile(transform.position);

            m_bucket.bucketGoDown = false;
            m_bucket.releaseWater = false;

            if (m_myCurrentTile.GetTileType() == TileType.Water)
            {
                PickUpWater(m_myCurrentTile);
            }
            else
            {
                DropWater(m_myCurrentTile);
            }
        }
        else if (!m_attackDown || m_crashed)
        {
            m_bucket.bucketGoDown = false;
            m_bucket.releaseWater = false;
        }

        if (m_disableFlying)
        {
            m_outOfBoundsTimer += Time.deltaTime;
            if(m_outOfBoundsTimer > 5f)
            {
                ResetRespawn();
                m_outOfBoundsTimer = 0;
            }
            m_rigidBody.linearVelocity = Vector3.zero;
            m_rigidBody.angularVelocity = Vector3.zero;
            transform.RotateAround(m_pivotObject.transform.position, new Vector3(0, 1, 0), m_rotationSpeed * Time.deltaTime);
        }


        //if (m_attackDown)
        //{
        //    m_interactDown = false;
        //    DropWater(m_myCurrentTile);
        //}
        //else if (m_bucket.releaseWater)
        //{
        //    m_bucket.releaseWater = false;
        //}

        OutOfBounds();
        SetInput();

        if (m_interactDown && !m_rotorTipTrailController.IsEmitting)
            m_rotorTipTrailController.SetIsEmitting(true);
        else if (!m_interactDown && m_rotorTipTrailController.IsEmitting)
            m_rotorTipTrailController.SetIsEmitting(false);

        m_rotorSpeed = Mathf.Lerp(m_rotorSpeed, m_interactDown ? m_maxRotorRotationSpeed : m_minRotorRotationSpeed, 1f - Mathf.Exp(-5f * Time.deltaTime));
        m_rotorRotation += m_rotorSpeed * Time.deltaTime;
        m_rotorTransform.rotation = Quaternion.Euler(0, m_rotorRotation, 0); //Spins the rotars of the helicopter
        m_tailRotorTransform.rotation = Quaternion.Euler(m_rotorRotation, 0f, 0f);

        float rotorPitchTarget = 1f;
        float linearVelocityMagnitude = m_rigidBody.linearVelocity.magnitude;
        m_linearVelocityMagnitude = linearVelocityMagnitude;

        if (m_interactDown)
        {
            rotorPitchTarget = m_rotorPitchController.maxRotorPitch;
        }
        else if (linearVelocityMagnitude < m_sprintSpeed && linearVelocityMagnitude > m_moveSpeed)
        {
            rotorPitchTarget = m_rotorPitchController.defaultRotorPitch;
        }
        else if (linearVelocityMagnitude < m_linearVelocityMinPitchThreshold)
        {
            float inverseVelocityLerp = Mathf.InverseLerp(0f, m_linearVelocityMinPitchThreshold, linearVelocityMagnitude);
            rotorPitchTarget = Mathf.Lerp(m_rotorPitchController.minRotorPitch, m_rotorPitchController.defaultRotorPitch, inverseVelocityLerp);
        }

        m_rotorPitchController.UpdatePitchTarget(rotorPitchTarget);

        if (m_crashed)
        {
            print("Crashing");
            SoundManager.StopRotorLoop();
            m_rotorTipTrailController.SetIsActive(false);
            m_fallTimer += Time.deltaTime;
            // Do not uncomment this \/
            if (m_crashed && !hasImpactPlayed)
            {
                SoundManager.PlayHeliImpact();
                hasImpactPlayed = true;
            }
            if (m_fallTimer >= 3)
            {
                m_fallTimer = 0f;
                SoundManager.PlayRotorLoop();
                hasImpactPlayed = false;
                Respawn();
            }
        }
        if (m_playerValues.m_water <= 0)
        {
            m_bucket.releaseWater = false;
        }
    }

    public void FixedUpdateHeli()
    {
        MoveHelicopter();
        RotateHelicopter();      
    }

    private void SetInput()
    {
        if (m_moveInput != Vector2.zero)
        {
            m_lookInput = Vector2.Lerp(m_lookInput, m_moveInput, 1f - Mathf.Exp(-m_lookInputLerpSpeed * Time.deltaTime));
        }
    }

    private bool m_interactDown = false;
    private void OnInteract()
    {
         //m_interactDown = inputValue.isPressed;
        
        m_interactDown =! m_interactDown;
    }

    private bool m_attackDown = false;
    private void OnAttack()
    {
        m_attackDown = !m_attackDown;
    }

    public bool GetAttackDown()
    {
        return m_attackDown;
    }

    public void OnMove(InputValue input)
    {
        m_moveInput = input.Get<Vector2>();
        //Debug.Log("Moving");
    } 

    public Vector2 GetMovement()
    {
        return m_moveInput;
    }

    public Rigidbody GetRRigidBody()
    {
        return m_rigidBody;
    }

    private void MoveHelicopter()
    {
        if(!m_disableFlying)
        {
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(m_camera.transform.rotation * Vector3.forward, Vector3.up).normalized;

            if (cameraPlanarDirection.sqrMagnitude == 0f)
                cameraPlanarDirection = Vector3.ProjectOnPlane(m_camera.transform.rotation * Vector3.up, Vector3.up).normalized;

            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Vector3.up);
            Vector3 lookDirection = new Vector3(m_lookInput.x, 0f, m_lookInput.y);
            Vector3 lookDirectionVector = cameraPlanarRotation * lookDirection;

            Vector3 moveInputVector = cameraPlanarRotation * new Vector3(m_moveInput.x, 0f, m_moveInput.y);

            Vector3 targetVelocity = moveInputVector;

            targetVelocity = Vector3.ClampMagnitude(targetVelocity, 1f);
            targetVelocity = targetVelocity * (m_interactDown ? m_sprintSpeed : m_moveSpeed);

            m_rigidBody.linearVelocity = Vector3.Lerp(m_rigidBody.linearVelocity, targetVelocity, 1f - Mathf.Exp(-(m_interactDown ? m_sprintSharpness : m_moveSharpness) * Time.deltaTime));
        }
        
        
    }
    private void RotateHelicopter()
    {
        if(!m_disableFlying)
        {
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(m_camera.transform.rotation * Vector3.forward, Vector3.up).normalized;

            if (cameraPlanarDirection.sqrMagnitude == 0f)
                cameraPlanarDirection = Vector3.ProjectOnPlane(m_camera.transform.rotation * Vector3.up, Vector3.up).normalized;

            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Vector3.up);
            Vector3 lookDirection = new Vector3(m_lookInput.x, 0f, m_lookInput.y);
            Vector3 lookDirectionVector = cameraPlanarRotation * lookDirection;

            Quaternion targetRotation = transform.rotation;
            if (lookDirectionVector.sqrMagnitude > 0f)
            {
                targetRotation = Quaternion.LookRotation(lookDirectionVector, Vector3.up);
            }
            m_rigidBody.rotation = Quaternion.Slerp(m_rigidBody.rotation, targetRotation, 1f - Mathf.Exp(-m_rotateSharpness * Time.deltaTime));
        }
        
    }

    private void PickUpWater(Tile p_currentTile) //It also cooperates with getting water
    {
        if (p_currentTile.GetTileType() == TileType.Water && m_playerValues.m_water < 100)
        {
            m_bucket.bucketGoDown = true;
            _ = m_playerValues.m_water += m_waterPickupSpeed * Time.deltaTime;
            print("GetWater");
        }
    }

    private void DropWater(Tile p_currentTile) //This function will extinguish the areas that burn if the water hits them
    {
        if (m_playerValues.m_water > 0)
        {
            m_bucket.releaseWater = true;
            _ = m_playerValues.m_water -= m_waterDropSpeed * Time.deltaTime;
            List<int> t = m_gridManager.TryGetTileIndexInRange(transform.position, m_waterReach);
            m_gridManager.OnWaterComing(t);
            print("DropWater");
        }
    }

    private void OutOfBounds() //This might drop some frames, will come up with better solution
    {
        Tile m_myCurrentTile = m_gridManager.TryGetTile(transform.position);
        if (m_myCurrentTile == null)
        {
            m_disableFlying = true;
            print("TurnBack");
        }
        else
        {
            m_outOfBoundsTimer = 0;
            m_disableFlying = false;
        }
    }

    private void Respawn() //This repawns the player at chosen location
    {
        m_rotorTipTrailController.SetIsActive(true);
        PrototypeCamera.SetFollowTransform(transform);
        transform.position = new Vector3(m_positionHistory[0].x, 15, m_positionHistory[0].y);
        m_positionHistory.RemoveRange(1, m_positionHistory.Count - 1);
        foreach (Renderer r in m_meshrenders)
        {
            r.enabled = true;
        }
        m_enableFlying = true;
        m_crashed = false;
        m_rigidBody.isKinematic = false;
        m_rigidBody.linearVelocity = Vector3.zero;
        m_rigidBody.angularVelocity = Vector3.zero;
    }

    private void ResetRespawn()
    {
        PrototypeCamera.SetFollowTransform(transform);
        transform.position = m_spawnPoint.transform.position;
        foreach (Renderer r in m_meshrenders)
        {
            r.enabled = true;
        }
        m_enableFlying = true;
        m_crashed = false;
        m_rigidBody.isKinematic = false;
        m_rigidBody.linearVelocity = Vector3.zero;
        m_rigidBody.angularVelocity = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach(var point in m_positionHistory)
        {
            Gizmos.DrawSphere(new Vector3(point.x, 15, point.y), 0.5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {     
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("BigTree") && m_crashed != true) //If the helicopter hits a tree, it will not be able to fly and will crash
        {
            print("HitTree ):");
            m_enableFlying = false;
            m_crashed = true;
            m_rigidBody.isKinematic = true;
            
            foreach (Renderer r in m_meshrenders)
            {
                r.enabled = false;
            }
            if(Instantiate(m_crashedHelicopterPrefab, transform.position, transform.rotation).TryGetComponent(out CrashedHelicopter crashedHelicopter))
            {
                crashedHelicopter.InitializeCrash(m_rigidBody.linearVelocity);            
            }         
        }
    }

    private IEnumerator UpdatePositionHistory()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_positionHistoryUpdateDelay);

            if (m_positionHistory.Count > m_maxHistoryEntries) m_positionHistory.RemoveAt(0);
            //Debug.Log(m_positionHistory.Count);

            m_positionHistory.Add(new Vector2(transform.position.x, transform.position.z));
        }
    }
}
