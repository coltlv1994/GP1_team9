using UnityEngine;
using UnityEngine.VFX;

public class Bucket : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    public VisualEffect m_waterEffect;
    public ParticleSystem m_WaterParticleSystem;
    private AudioSource waterSource;

    public bool bucketGoDown = false;
    public bool releaseWater = false;

    public float m_stopWaterDelay = .25f;
    private float m_stopWaterTimer = 0f;

    private void Start()
    {
        waterSource = SoundManager.GetWaterSource();
    }

    private void Update()
    {
        if (bucketGoDown && !m_animator.GetBool("BucketDown"))
        {
            SoundManager.PlayWaterPickup();
            m_animator.SetBool("BucketDown", true);
        }
        else if (!bucketGoDown && m_animator.GetBool("BucketDown"))
        {
            m_animator.SetBool("BucketDown", false);
        }

        if (releaseWater)
        {
            m_stopWaterTimer = m_stopWaterDelay;

            if (!m_waterEffect.enabled)
            {
                m_WaterParticleSystem.Play();
                m_waterEffect.enabled = true;
                m_waterEffect.Play();
                SoundManager.PlayWaterDrop();
            }

            waterSource.volume = Mathf.Lerp(waterSource.volume, 1f, 1f - Mathf.Exp(-5f * Time.deltaTime));
        }
        else if (!releaseWater && m_waterEffect.enabled)
        {
            m_stopWaterTimer -= Time.deltaTime;

            if (m_stopWaterTimer <= 0f)
            {
                m_WaterParticleSystem.Stop();
                m_waterEffect.enabled = false;
                m_waterEffect.Stop();
            }
        }
        else if (!releaseWater)
        {
            waterSource.volume = Mathf.Lerp(waterSource.volume, 0f, 1f - Mathf.Exp(-10f * Time.deltaTime));
        }
    }
}
