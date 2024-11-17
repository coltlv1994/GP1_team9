using Grid;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayState : State
{
#if UNITY_EDITOR
    [SerializeField, HideInInspector] private string m_initialLevel;
#endif

    HeliCopterScript player;

#if UNITY_EDITOR
    public GameplayState()
    {
        m_initialLevel = "GymLevel_Artists";
    }

    public GameplayState(string p_initialLevel)
    {
        m_initialLevel = p_initialLevel;
    }
#endif

    public void OnEnter()
    {
        m_isPaused = true;
#if UNITY_EDITOR
        SceneManager.LoadSceneAsync(m_initialLevel, LoadSceneMode.Additive);
#elif UNITY_STANDALONE
        SceneManager.LoadSceneAsync("GymLevel_Artists", LoadSceneMode.Additive);
#endif
    }

    public void OnSceneLoaded(Scene p_scene, LoadSceneMode p_loadMode)
    {
        m_gridManager = GridManager.GetInstance();

        m_fireTickTimer = 1.0f / (m_fireTickRate * PlayerPrefs.GetFloat("Difficulty"));
        m_timeBeforeFire = m_fireTickTimer;

        m_waterTickTimer = 1.0f / m_waterTickRate;
        m_timeBeforeWater = m_waterTickTimer;

        m_indicatoTickTimer = 1.0f / m_indicatorUpdateTickRate;
        m_timeBeforeUpdate = m_indicatoTickTimer;

        m_gridManager.InitFireTile();

        player = GameObject.Find("Helicopter").GetComponent<HeliCopterScript>();

        SceneManager.SetActiveScene(p_scene);
        m_isPaused = false;
    }

    public void Update()
    {
        if (m_isPaused) return;

        // move GridManager's update to here?
        m_timeBeforeFire -= Time.deltaTime;
        m_timeBeforeWater -= Time.deltaTime;
        m_timeBeforeUpdate -= Time.deltaTime;

        bool spreadFire = (m_timeBeforeFire < 0);
        bool updateIndicator = m_timeBeforeUpdate < 0;

        if (spreadFire)
        {
            // call fire logic after water
            if(m_gridManager.FireSpread())
            {
                // returns true means the big tree has been on fire
                // game loses

                List<SoundManager.DialogueName> loseDialogue = new List<SoundManager.DialogueName>() {
                    SoundManager.DialogueName.LOSE01,
                    SoundManager.DialogueName.LOSE02,
                    SoundManager.DialogueName.LOSE03 };

                SoundManager.PlayDialogue(loseDialogue[Random.Range(0, loseDialogue.Count)]);
                SoundManager.StopRotorLoop();
                SoundManager.StopFireSource();
                SoundManager.StopWaterLoop();

                PlayerPrefs.SetInt("PlayerSucceeded", 0);
                GameManager.GetInstance().ChangeState(new EndMenuState());
            }
            m_timeBeforeFire = m_fireTickTimer;
        }

        if(updateIndicator)
        {
            GridManager.GetInstance().EvaluateFireGroups();
            m_timeBeforeUpdate = m_indicatoTickTimer;
        }

        if (GridManager.GetInstance().FireHaveBeenPutOut())
        {
            List<SoundManager.DialogueName> winDialogue = new List<SoundManager.DialogueName>() {
                SoundManager.DialogueName.WIN01,
                SoundManager.DialogueName.WIN02 };

            SoundManager.PlayDialogue(winDialogue[Random.Range(0, winDialogue.Count)]);
            SoundManager.StopRotorLoop();
            SoundManager.StopFireSource();
            SoundManager.StopWaterLoop();

            PlayerPrefs.SetInt("PlayerSucceeded", 1);
            GameManager.GetInstance().ChangeState(new EndMenuState());
        }

        player.UpdateHeli();
    }

    public void FixedUpdate()
    {
        player.FixedUpdateHeli();
    }

    public void OnExit()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }

    private GridManager m_gridManager = null;
    private float m_fireTickRate = 0.75f;
    private float m_fireTickTimer = 0.0f;
    private float m_timeBeforeFire = 0.0f;

    private float m_waterTickRate = 2f;
    private float m_waterTickTimer = 0.0f;
    private float m_timeBeforeWater = 0.0f;

    private float m_indicatorUpdateTickRate = 1;
    private float m_indicatoTickTimer;
    private float m_timeBeforeUpdate;

    private bool m_isPaused = false;
}
