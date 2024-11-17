using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenuState : State
{
    public void OnEnter()
    {
        SceneManager.LoadSceneAsync("EndMenu", LoadSceneMode.Additive);
    }

    public void Update()
    {

    }

    public void OnExit()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
}
