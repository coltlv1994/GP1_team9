using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class WaterIndicatorUI : MonoBehaviour
{
    new public Camera m_camera;
    public Transform m_helicopterTransform;
    public List<Transform> m_waterTransforms;
    public List<RectTransform> m_waterIndicators;

    [Header("Settings")]
    public float m_indicatorCircleRadius = 300f;

    [SerializeField] private GameObject m_waterIndicatorPrefab;
    private float m_findWaterTimer = 5f;
    [SerializeField] private PlayerValues m_playerValues;
    public float m_lowestWaterValue;

    private void Awake()
    {
        if (m_camera == null)
        {
            m_camera = GameObject.FindFirstObjectByType<Camera>();
        }
    }
    private void Start()
    {

        foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("Water"))
        {
            m_waterTransforms.Add(fooObj.transform);
        }
        for (int i = 0; i < m_waterTransforms.Count; i++)
        {
            GameObject waterIndicator = Instantiate(m_waterIndicatorPrefab, transform);
            RectTransform waterIndicatorRect = waterIndicator.GetComponent<RectTransform>();
            if (!m_waterIndicators.Contains(waterIndicatorRect))
            {
                m_waterIndicators.Add(waterIndicatorRect);
            }
        }
        
    }

    private void Update()
    {
            m_findWaterTimer += Time.deltaTime; //Moved this whole thing from start, this might get changed for performance
        if (m_findWaterTimer >= 5)
        {
            if (m_playerValues.m_water <= m_lowestWaterValue)
            {
                foreach (RectTransform m_waterOBJS in m_waterIndicators)
                {
                    m_waterOBJS.gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (RectTransform m_waterOBJS in m_waterIndicators)
                {
                    m_waterOBJS.gameObject.SetActive(false);
                }
            }
            m_findWaterTimer = 0;
        }

        for (int i = 0; i < m_waterIndicators.Count; i++)
        {
            UpdateTransformScreenPosition(m_waterIndicators[i], m_waterTransforms[i]);
        }
    }

    private void UpdateTransformScreenPosition(RectTransform p_rect, Transform p_transform)
    {
        Vector3 helicopterPosition = m_helicopterTransform.position;
        helicopterPosition.y = 0f;

        Vector3 waterPosition = p_transform.position;
        waterPosition.y = 0f;

        Vector3 directionFromHelicopterToWater = (waterPosition - helicopterPosition);
        float indicatorDistance = directionFromHelicopterToWater.magnitude;
        indicatorDistance = Mathf.Clamp(indicatorDistance, 0f, m_indicatorCircleRadius);

        Vector3 clampedIndicatorPosition = helicopterPosition + directionFromHelicopterToWater.normalized * indicatorDistance;

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
