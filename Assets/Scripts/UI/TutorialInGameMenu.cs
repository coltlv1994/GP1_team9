using UnityEngine;

public class TutorialInGameMenu : InGameMenuScript
{
    [SerializeField] private AudioSource m_source;

    public override void PauseGame(bool p_pause)
    {
        base.PauseGame(p_pause);

        if (p_pause)
            m_source.Pause();
        else
            m_source.Play();
    }
}
