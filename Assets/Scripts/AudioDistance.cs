using Grid;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioDistance : MonoBehaviour
{
    [SerializeField] AudioSource m_Source;
    private float m_findFireTimer;
    public float m_maxVolume = 10f;
    public float m_findClosestFireRefreshTimer;

    public GridManager m_gridManager = null;
    public GameObject m_closestFire;
    public GameObject m_helicopterOBJ;
    void Start()
    {
        m_findFireTimer = m_findClosestFireRefreshTimer;
        FindClosestFire();
        m_gridManager = GridManager.GetInstance();
        StartCoroutine(AdjustVolume());
    }

    public GameObject FindClosestFire() //Finds all the objects with the Fire Tag on it and checks which one is closest
    {
        GameObject[] fires;
        fires = GameObject.FindGameObjectsWithTag("Fire");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = m_helicopterOBJ.transform.position;
        foreach (GameObject go in fires)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        m_closestFire = closest;
        return closest;
    }
    
    private void Update()
    {
           m_findFireTimer += Time.deltaTime;
        if (m_findFireTimer >= m_findClosestFireRefreshTimer)
        {
            FindClosestFire();
            m_findFireTimer = 0;
        }
    }

    IEnumerator AdjustVolume()
    {
        while (true)
        {
          if (m_Source.isPlaying)
          { // do this only if some audio is being played in this gameObject's AudioSource

                float distanceToTarget = Vector3.Distance(m_helicopterOBJ.transform.position, m_closestFire.transform.position); // Assuming that the target is the player or the audio listener

                if (distanceToTarget < 1) { distanceToTarget = 1; }

                m_Source.volume = m_maxVolume / distanceToTarget; // this works as a linear function, while the 3D sound works like a logarithmic function, so the effect will be a little different (correct me if I'm wrong)

                yield return new WaitForSeconds(1); // this will adjust the volume based on distance every 1 second (Obviously, You can reduce this to a lower value if you want more updates per second)


          }
        }
    }
}
