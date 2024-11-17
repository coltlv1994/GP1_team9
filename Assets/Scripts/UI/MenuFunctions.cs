using UnityEngine;

public class MenuFunctions : MonoBehaviour
{
    public void ToGamePlay()
    {
        GameManager.GetInstance().ChangeState(new GameplayState());
        Time.timeScale = 1;
    }

    public void ToMainMenu()
    {
        GameManager.GetInstance().ChangeState(new MainMenuState());
        Time.timeScale = 1;
    }
}
