using System.Collections;
using UnityEngine;

public class GeyserScript : MonoBehaviour
{
    float m_timer;
    int m_random;
    Color normal; //Remove this later. Since there is no animation for the geyser this is what shows that it is active.

    bool touch;
    public GameObject ScriptHolder; //Whatever gameobject has the playervalues script
    PlayerValues PlayerValues;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        normal = gameObject.GetComponent<MeshRenderer>().material.color;
        m_random = Random.Range(6, 10);
        PlayerValues = ScriptHolder.GetComponent<PlayerValues>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_timer >= m_random)
        {
            StartCoroutine(startBehaviour());
        }
        else
        {
            m_timer += Time.deltaTime;
        }

        if(touch)
        {
            PlayerValues.m_water++;
        }
    }

    IEnumerator startBehaviour()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(3);
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().material.color = normal;
        m_random = Random.Range(6, 10);
        m_timer = 0;
        touch = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            touch = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            touch = false;
        }
    }
}
