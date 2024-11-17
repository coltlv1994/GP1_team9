using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerValues : MonoBehaviour
{
    [SerializeField] Slider WaterSlider;
    public float m_water;
    public bool m_startFulllWater = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_startFulllWater)
        {
            m_water = 100;
        }
        else
        {
            m_water = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        WaterSlider.value = m_water;

        if (m_water <= 0)
        {
            m_water = 0;
        }
        else if (m_water >= 100)
        {
            m_water = 100;
        }
    }
}