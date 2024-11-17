using UnityEngine;

public class DestroyIndicator : MonoBehaviour
{

    public FireIndicatorUI m_fireIndicator;
    private void Start()
    {
        m_fireIndicator = GetComponentInParent<FireIndicatorUI>();
    }

    private void Update()
    {
        if(m_fireIndicator.m_findFireTimer == 0) //This might need some improvements
        {
            Destroy(gameObject);
        }
    }
}
