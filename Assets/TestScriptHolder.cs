using UnityEngine;
using static SoundManager;

public class TestScriptHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.PlayGameMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SoundManager.PlayDialogue(DialogueName.WIN01);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SoundManager.PlayDialogue(DialogueName.LOSE01);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SoundManager.PlayDialogue(DialogueName.STORY01);
        }
    }
}
