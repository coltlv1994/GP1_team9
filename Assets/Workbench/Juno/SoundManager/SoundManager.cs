using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip rotorLoopClip;
    [SerializeField] private AudioClip waterDropClip;
    [SerializeField] private AudioClip waterPickupClip;
    [SerializeField] private AudioClip waterPshhClip;
    [SerializeField] private AudioClip heliImpactClip;
    [SerializeField] private AudioClip heliExplodeClip;
    [SerializeField] private AudioClip heliRareExplodeClip;

    [SerializeField] private AudioClip[] dialogueList;

    [SerializeField] private AudioClip menuMusicClip;
    [SerializeField] private AudioClip gameMusicClip;

    // Audio players components.
    [SerializeField] private AudioSource EffectsSource;
    [SerializeField] private AudioSource MusicSource;
    [SerializeField] private AudioSource RotorSource;
    [SerializeField] private AudioSource VoiceSource;
    [SerializeField] private AudioSource FireSource;
    [SerializeField] private AudioSource WaterSource;

    // Random pitch adjustment range.
    [SerializeField] private float LowPitchRange = .95f;
    [SerializeField] private float HighPitchRange = 1.05f;

    public enum DialogueName
    {
        TUTORIAL01,
        TUTORIAL02,
        TUTORIAL03,
        TUTORIAL04,
        TUTORIAL05,
        TUTORIAL06,
        TUTORIAL07,
        TUTORIAL08,
        STORY01,
        STORY02,
        STORY03,
        STORY04,
        STORY05,
        STORY06,
        STORY07,
        STORY08,
        CLOSETOCRASH01,
        CLOSETOCRASH02,
        CRASH01,
        CRASH02,
        TURNAROUND01,
        NEARVILLAGE01,
        NEARVILLAGE02,
        NEARVILLAGE03,
        WIN01,
        WIN02,
        LOSE01,
        LOSE02,
        LOSE03
    }

    // Singleton instance.
    public static SoundManager Instance = null;

    public static AudioSource GetFireSource()
    {
        return Instance.FireSource;
    }

    public static AudioSource GetWaterSource()
    {
        return Instance.WaterSource;
    }

    public static void StopFireSource()
    {
        if (Instance == null)
            return;

        Instance.FireSource.Stop();
    }

    public static void PlayFireLoop()
    {
        Instance.FireSource.Play();
    }

    public static void StopWaterLoop()
    {
        if (Instance == null)
            return;

        Instance.WaterSource.Stop();
    }

    public static void PlayWaterLoop()
    {
        Instance.WaterSource.Play();
    }

    public static void PlayerWaterPshh()
    {
        Instance.EffectsSource.PlayOneShot(Instance.waterPshhClip);
    }

    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Play a sound effect with "SoundManager.FunctionName"
    public static void PlayWaterDrop()
    {
        Instance.EffectsSource.PlayOneShot(Instance.waterDropClip);
    }
    public static void PlayWaterPickup()
    {
        Instance.EffectsSource.PlayOneShot(Instance.waterPickupClip);
    }
    public static void PlayHeliExplode()
    {
        Instance.EffectsSource.PlayOneShot(Instance.heliExplodeClip);
    }
    public static void PlayHeliImpact()
    {
        if (Random.Range(0, 100) <= 75)
        {
            Instance.EffectsSource.PlayOneShot(Instance.heliImpactClip);
        }
        else
        {
            Instance.EffectsSource.PlayOneShot(Instance.heliRareExplodeClip);
        }
    }
    public static AudioSource GetRotorSource()
    {
        return Instance.RotorSource;
    }
    public static void PlayRotorLoop() // Put this in the helicopter controller or something idk
    {
        Instance.RotorSource.clip = Instance.rotorLoopClip;
        Instance.RotorSource.Play();
    }
    public static void StopRotorLoop()
    {
        if (Instance == null)
            return;

        Instance.RotorSource.clip = Instance.rotorLoopClip;
        Instance.RotorSource.Stop();
    }

    public static void PlayMenuMusic()
    {
        Instance.MusicSource.Stop();
        Instance.MusicSource.clip = Instance.menuMusicClip;
        Instance.MusicSource.Play();
    }
    public static void PlayGameMusic()
    {
        Instance.MusicSource.Stop();
        Instance.MusicSource.clip = Instance.gameMusicClip;
        Instance.MusicSource.Play();
    }

    // Play any sound from the dialogue list with: SoundManager.PlayDialogue(DialogueName.NAME);
    public static void PlayDialogue(DialogueName sound, float volume = 1)
    {
        Instance.VoiceSource.PlayOneShot(Instance.dialogueList[(int)sound]);
    }

    // Play a random clip from an array, and randomize the pitch slightly.
    public static void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(Instance.LowPitchRange, Instance.HighPitchRange);

        Instance.EffectsSource.pitch = randomPitch;
        Instance.EffectsSource.clip = clips[randomIndex];
        Instance.EffectsSource.Play();
    }
}