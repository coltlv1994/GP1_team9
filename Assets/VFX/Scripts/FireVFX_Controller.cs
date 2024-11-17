using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class FireVFX_Controller : MonoBehaviour
{
    public static float m_maxFire = 0.1f;
    public MeshRenderer meshRenderer;
    public VisualEffect VFXGraph;
    public float fireRate = 0.0075f;
    public float refreshRate = 0.025f;

    private Material[] meshMaterial;

    void Awake()
    {
        if(meshRenderer == null)
        {
            Debug.Log("Test");
        }
        if (meshRenderer != null)
            meshMaterial = meshRenderer.materials;

        if (meshMaterial == null)
        {
            Debug.Log("ddsds");
        }
    }

    private void Start()
    {
        for (int i = 0; i < meshMaterial.Length; i++)
        {
            meshMaterial[i].SetFloat("_FireEdge_Width", 0);
        }
    }

    public void StartFire()
    {
        StartCoroutine(FireCo());
    }

    public void StopFire()
    {
        StopAllCoroutines();
        for (int i = 0; i < meshMaterial.Length; i++)
        {
            meshMaterial[i].SetFloat("_FireAmount", 0);
            meshMaterial[i].SetFloat("_FireEdge_Width", 0);
        }
        if (VFXGraph != null)
        {
            VFXGraph.Stop();
        }
    }

    IEnumerator FireCo()
    {
        if (VFXGraph != null)
        {
            VFXGraph.Play();
        }

        if (meshMaterial == null)
        {
            Debug.Log("kjhkjhkjh");
        }

        if (meshMaterial.Length > 0)
        {
            float counter = 0;

            while (meshMaterial[0].GetFloat("_FireAmount") < 1)
            {
                counter += fireRate;

                counter = Mathf.Min(counter, m_maxFire);

                for (int i = 0; i < meshMaterial.Length; i++)
                {
                    meshMaterial[i].SetFloat("_FireAmount", counter);
                    meshMaterial[i].SetFloat("_FireEdge_Width", 0.01f);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }

    IEnumerator StopFireCo()
    {
        if (meshMaterial.Length > 0)
        {
            float counter = 0;

            while (meshMaterial[0].GetFloat("_FireAmount") > 0)
            {
                counter = meshMaterial[0].GetFloat("_FireAmount");
                counter -= fireRate;

                counter = Mathf.Max(counter, 0);

                for (int i = 0; i < meshMaterial.Length; i++)
                {
                    meshMaterial[i].SetFloat("_FireAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }


    }
}
