using UnityEngine;
using UnityEngine.UI;

public class MenuBackgroundSwitcher : MonoBehaviour
{
    //This script was made so that there would be less issues when using the menu script in the actual game scene

    [SerializeField] GameObject Background1, Background2;

    void Start()
    {
        Background1.SetActive(true);
        Background2.SetActive(false);
    }

    public void Settings()
    {
        Background1.SetActive(false);
        Background2.SetActive(true);
    }

    public void BackToMenu()
    {
        Background1.SetActive(true);
        Background2.SetActive(false);
    }


}
