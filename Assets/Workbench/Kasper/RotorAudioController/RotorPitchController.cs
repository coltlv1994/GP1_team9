using UnityEngine;

public class RotorPitchController : MonoBehaviour
{
    public float minRotorPitch = .9f, defaultRotorPitch = 1f, maxRotorPitch = 1.5f;
    public float pitchChangeSharpness = 5f;
    private float targetRotorPitch = 1f;
    private AudioSource rotorSource;

    private void Start()
    {
        rotorSource = SoundManager.GetRotorSource();
    }

    private void Update()
    {
        rotorSource.pitch = Mathf.Lerp(rotorSource.pitch, targetRotorPitch, 1f - Mathf.Exp(-pitchChangeSharpness * Time.deltaTime));
    }

    public void UpdatePitchTarget(float value)
    {
        targetRotorPitch = value;
    }
}
