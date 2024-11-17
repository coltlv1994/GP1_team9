using Dialogue;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuScript : MonoBehaviour
{
    public virtual void PauseGame(bool p_pause)
    {
        Time.timeScale = p_pause ? 0 : 1;
    }

    public virtual void Restart()
    {
        GameManager.GetInstance().ChangeState(new GameplayState());
        Time.timeScale = 1;
    }

    public virtual void ReturnToMainMenu()
    {
        GameManager.GetInstance().ChangeState(new MainMenuState());
        Time.timeScale = 1;
    }

    public void ChangeLanguage(int p_language)
    {
        Dialogue.DialogueManager.GetInstance().ChangeLanguage((Language)p_language);
    }
}
