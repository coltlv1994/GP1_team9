using Grid;
using System.Collections.Generic;
using UnityEngine;

public class FireIndicatorUI : MonoBehaviour
{
    new public Camera m_camera;
    public Transform m_helicopterTransform;
    public List<GameObject> m_fireIndicators = new List<GameObject>();
    public List<RectTransform> m_fireIndicatorTransform = new List<RectTransform>();
    [SerializeField] private int m_maxIcons;

    [Header("Settings")]
    public float m_indicatorCircleRadius = 300f;

    [SerializeField] private GameObject m_fireIndicatorPrefab;
    public float m_findFireTimer = 5f;
    private float m_updateTimer = 0;

    private void Awake()
    {
        if (m_camera == null)
        {
            m_camera = GameObject.FindFirstObjectByType<Camera>();
        }

        for(int i = 0;i < m_maxIcons; i++)
        {
            GameObject newIndicator = Instantiate(m_fireIndicatorPrefab, transform);
            newIndicator.SetActive(false);
            m_fireIndicatorTransform.Add(newIndicator.GetComponent<RectTransform>());
            m_fireIndicators.Add(newIndicator);
        }
    }

    private void Update()
    {
        m_updateTimer += Time.deltaTime; //Moved this whole thing from start, this might get changed for performance
        if(m_updateTimer >= m_findFireTimer)
        {
            foreach (GameObject indicator in m_fireIndicators)
                indicator.SetActive(false);

            int index = 0;
            foreach (var group in GridManager.GetInstance().m_fireGroups)
            {
                if (index == m_maxIcons) break;

                m_fireIndicators[index].SetActive(true);

                UpdateTransformScreenPosition(m_fireIndicatorTransform[index], group.GetCenter());

                index++;
            }

            m_updateTimer = 0;
        }
    }

    private void UpdateTransformScreenPosition(RectTransform p_rect, Vector3 p_position)
    {
        Vector3 helicopterPosition = m_helicopterTransform.position;
        helicopterPosition.y = 0f;

        p_position.y = 0;

        Vector3 directionFromHelicopterToFire = (p_position - helicopterPosition);
        float indicatorDistance = directionFromHelicopterToFire.magnitude;
        indicatorDistance = Mathf.Clamp(indicatorDistance, 0f, m_indicatorCircleRadius);

        Vector3 clampedIndicatorPosition = helicopterPosition + directionFromHelicopterToFire.normalized * indicatorDistance;

        Vector3 screenPosition = m_camera.WorldToScreenPoint(clampedIndicatorPosition);
        Vector3 helicopterScreenPosition = m_camera.WorldToScreenPoint(helicopterPosition);

        Vector3 screenDirectionToIndicator = screenPosition - helicopterScreenPosition;
        Quaternion screenRotation = Quaternion.LookRotation(screenDirectionToIndicator);
        float angle = AngleSigned(Vector3.up, screenDirectionToIndicator, Vector3.forward);

        p_rect.position = screenPosition;
        p_rect.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }
}
