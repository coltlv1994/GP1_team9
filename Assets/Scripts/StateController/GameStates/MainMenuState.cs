using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : State
{
    public void OnEnter()
    {
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
    }

    public void Update()
    {

    }

    public void OnExit()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
}
