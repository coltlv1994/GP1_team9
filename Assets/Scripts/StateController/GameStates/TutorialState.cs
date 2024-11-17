using Dialogue;
using Grid;
using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TutorialStage
{
    PlayDialogue,
    Movement,
    PickWater,
    DropWater,
    Finish
}

public class TutorialState : State
{
    [SerializeField] private float m_firstLineDelay;
    private TutorialStage stage = TutorialStage.PlayDialogue;
    private TutorialStage stagePassed = TutorialStage.PlayDialogue;
    bool havePressed = false;
    int m_tileToLight;
    HeliCopterScript player;
    OpenMenuScript openMenu;
    PlayerValues playerValues;
    GameObject m_finishedMenu;
    Vector3 m_movementIntroDestination;

    public void OnEnter() 
    {
        SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Additive);
    }

    public void OnSceneLoaded(Scene p_scene, LoadSceneMode p_loadMode)
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<HeliCopterScript>();
        openMenu = player.gameObject.GetComponent<OpenMenuScript>();
        playerValues = GameObject.Find("ScriptHolder").GetComponent<PlayerValues>();
        m_tileToLight = GridManager.GetInstance().TryGetTileIndexInRange(GameObject.Find("Tree3_Fire").transform.position, 0)[0];
        m_finishedMenu = GameObject.Find("Finish");
        m_finishedMenu.SetActive(false);
        m_movementIntroDestination = GameObject.Find("PointToReach").transform.position;
        Dialogue.DialogueManager.GetInstance().Init();
        Dialogue.DialogueManager.GetInstance().MoveToNextLine();
    }

    public void Update() 
    {
        if (stage == TutorialStage.Finish) return;

        if(player.GetAttackDown() && !openMenu.IsMenuOpened())
        {
            if (stage == TutorialStage.PlayDialogue && !havePressed)
            {
                Dialogue.DialogueManager.GetInstance().MoveToNextLine();

                havePressed = true;
            }
        }
        else
        {
            havePressed = false;
        }

        if (stage != TutorialStage.PlayDialogue)
        {
            player.UpdateHeli();

            switch (stage)
            {
                case TutorialStage.Movement:
                    if(Vector3.Distance(player.gameObject.transform.position, m_movementIntroDestination) < 4)
                    {
                        player.GetRRigidBody().isKinematic = true;
                        stage = TutorialStage.PlayDialogue;
                        DialogueManager.GetInstance().ChangeDialogue(1);
                        DialogueManager.GetInstance().MoveToNextLine();
                        GridManager.GetInstance().AddFire(new System.Collections.Generic.List<int>() { m_tileToLight });
                        stagePassed = TutorialStage.Movement;
                    }
                    break;
                case TutorialStage.PickWater:
                    if(playerValues.m_water == 100)
                    {
                        player.GetRRigidBody().isKinematic = true;
                        stage = TutorialStage.PlayDialogue;
                        DialogueManager.GetInstance().ChangeDialogue(2);
                        DialogueManager.GetInstance().MoveToNextLine();
                        stagePassed = TutorialStage.PickWater;
                    }
                    break;
                case TutorialStage.DropWater:
                    if(GridManager.GetInstance().FireHaveBeenPutOut())
                    {
                        player.GetRRigidBody().isKinematic = true;
                        stage = TutorialStage.PlayDialogue;
                        DialogueManager.GetInstance().ChangeDialogue(3);
                        DialogueManager.GetInstance().MoveToNextLine();
                        stagePassed = TutorialStage.DropWater;
                    }
                    break;
            }
        }
        else
        {
            if (DialogueManager.GetInstance().DialogueEnded() && DialogueManager.GetInstance().LineEnded())
            {
                switch(stagePassed)
                {
                    case TutorialStage.PlayDialogue:
                        stage = TutorialStage.Movement;
                        player.GetRRigidBody().isKinematic = false;
                        break;
                    case TutorialStage.Movement:
                        stage = TutorialStage.PickWater;
                        player.GetRRigidBody().isKinematic = false;
                        break;
                    case TutorialStage.PickWater:
                        stage = TutorialStage.DropWater;
                        player.GetRRigidBody().isKinematic = false;
                        break;
                    case TutorialStage.DropWater:
                        stage = TutorialStage.Finish;
                        m_finishedMenu.SetActive(true);
                        break;
                }
            }
        }
    }

    public void FixedUpdate()
    {
        if (stage == TutorialStage.Finish) return;

        if (stage != TutorialStage.PlayDialogue)
            player.FixedUpdateHeli();
    }

    public void OnExit() 
    {
        SceneManager.UnloadSceneAsync("Tutorial");
    }
}
