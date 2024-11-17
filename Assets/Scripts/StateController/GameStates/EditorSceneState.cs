#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
public class EditorSceneState : State
{
    public void OnEnter()
    {
        EditorSceneData so = AssetDatabase.LoadAssetAtPath<EditorSceneData>("Assets/Scripts/StateController/EditorSceneData.asset");
        switch (so.m_sceneName)
        {
            case "":
                GameManager.GetInstance().ChangeState(new MainMenuState());
                break;
            case "Root":
                GameManager.GetInstance().ChangeState(new MainMenuState());
                break;
            case "MainMenu":
                GameManager.GetInstance().ChangeState(new MainMenuState());
                break;
            case "EndMenu":
                GameManager.GetInstance().ChangeState(new EndMenuState());
                break;
            case "Tutorial":
                GameManager.GetInstance().ChangeState(new TutorialState());
                break;
            default:
                GameManager.GetInstance().ChangeState(new GameplayState(so.m_sceneName));
                break;
        }
    }
}
#endif