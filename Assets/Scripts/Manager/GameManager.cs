using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    // PUBLIC ------------------------------------------------------------
    public static GameManager GetInstance()

    {
        return m_instance;
    }


    public void ChangeState(State p_state)
    {
        m_stateController.SwitchState(p_state);
    }

    // PRIVATE -----------------------------------------------------------
    private StateController m_stateController = new StateController();
    private static GameManager m_instance;

    private void Awake()
    {
        if(m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        m_stateController.Init();
    }

    private void Start()
    {
#if UNITY_EDITOR
        ChangeState(new EditorSceneState());
        Debug.Log("Editor");
#elif UNITY_STANDALONE
        ChangeState(new MainMenuState());
        Debug.Log("Standalone");
#endif
    }

    private void Update()
    {
        m_stateController.UpdateState();
    }

    private void FixedUpdate()
    {
        m_stateController.FixedUpdateState();
    }
}
