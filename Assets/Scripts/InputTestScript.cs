using Grid;
using System;
using UnityEngine;

public class InputTestScript : MonoBehaviour
{
    void OnInteract()
    {
        Dialogue.DialogueManager.GetInstance().MoveToNextLine();
    }

    private void OnMove()
    {
        Debug.Log("tes");
    }

    void OnPause()
    {
        if (Dialogue.DialogueManager.GetInstance().m_language == Dialogue.Language.ENG) 
        {
            Dialogue.DialogueManager.GetInstance().ChangeLanguage(Dialogue.Language.SE);
        }
        else
        {
            Dialogue.DialogueManager.GetInstance().ChangeLanguage(Dialogue.Language.ENG);
        }
    }
}
