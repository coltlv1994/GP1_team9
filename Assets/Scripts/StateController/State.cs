using UnityEngine.SceneManagement;

public interface State
{
    public void OnEnter() { }
    public void OnSceneLoaded(Scene p_scene, LoadSceneMode p_loadMode) { }
    public void Update() { }
    public void FixedUpdate() { }
    public void OnExit() { }
}
