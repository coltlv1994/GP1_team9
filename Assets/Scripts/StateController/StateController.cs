using UnityEngine.SceneManagement;

public class StateController
{
    // PUBLIC ------------------------------------------
    public void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SwitchState(State p_newState)
    {
        m_isChanging = true;
        if (m_currentState != null) m_currentState.OnExit();
        m_nextState = p_newState;
        m_nextState.OnEnter();
    }

    public void OnSceneLoaded(Scene p_scene, LoadSceneMode p_loadMode)
    {
        m_currentState = m_nextState;
        m_currentState?.OnSceneLoaded(p_scene, p_loadMode);
        m_nextState = null;

        SceneManager.SetActiveScene(p_scene);
        m_isChanging = false;
    }

    public void UpdateState()
    {
        if (m_isChanging) return;
        m_currentState?.Update();
    }

    public void FixedUpdateState()
    {
        if (m_isChanging) return;
        m_currentState?.FixedUpdate();
    }

    // PRIVATE -----------------------------------------
    private State m_currentState;
    private State m_nextState;
    private bool m_isChanging = false;
}
