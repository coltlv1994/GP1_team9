using KasperPrototype;
using System.Collections.Generic;
using UnityEngine;

public class CrashedHelicopter : MonoBehaviour
{
    public bool m_explode = false;

    public float rotorSpeed = 50f;
    [SerializeField] private Rigidbody rotor;
    [SerializeField] private Rigidbody smallRotor;
    [SerializeField] private List<Rigidbody> m_bodyparts = new List<Rigidbody>();
    [SerializeField] private GameObject helicopterBody;
    [SerializeField] private GameObject miscBody;
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private ParticleSystem explosionEffect;

    [SerializeField] private float explosionForce = 50f;

    [SerializeField] private float explosionDelay = 3f;
    private float explosionTimer = 0f;

    public void InitializeCrash(Vector3 velocity)
    {
        PrototypeCamera.SetFollowTransform(helicopterBody.transform);
        Impact();
        SetVelocity(velocity);
    }

    public void SetVelocity(Vector3 velocity)
    {
        for (int i = 0; i < m_bodyparts.Count; i++)
        {
            m_bodyparts[i].linearVelocity = velocity;
        }
    }

    private void Update()
    {
        explosionTimer += Time.deltaTime;

        /*if (m_explode)
        {
            Impact();
            enabled = false;
        }*/

        if (helicopterBody.activeSelf && explosionTimer >= explosionDelay)
        {
            Explode();   
        }
    }

    private void Impact()
    {
        impactEffect.Play();
        miscBody.SetActive(false);
        rotor.angularVelocity = new Vector3(0f, rotorSpeed, 0f);
        smallRotor.angularVelocity = new Vector3(rotorSpeed, 0f, 0f);

        for (int i = 0; i < m_bodyparts.Count; i++)
        {
            m_bodyparts[i].useGravity = true;
            m_bodyparts[i].isKinematic = false;
        }
    }

    private void Explode()
    {
        SoundManager.PlayHeliExplode();
        impactEffect.transform.SetParent(null);
        impactEffect.Stop();
        explosionEffect.transform.SetParent(null);
        explosionEffect.transform.rotation = Quaternion.identity;
        explosionEffect.Play();
        helicopterBody.SetActive(false);

        for (int i = 0; i < m_bodyparts.Count; i++)
        {
            m_bodyparts[i].AddExplosionForce(explosionForce, explosionEffect.transform.position, 25f, explosionForce);
        }
        Destroy(gameObject, 5f);
    }
}
