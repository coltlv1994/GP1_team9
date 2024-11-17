using UnityEngine;

public class EndMenuScript : MenuFunctions
{
    [SerializeField] private GameObject m_winScreen;
    [SerializeField] private GameObject m_loseScreen;

    public void Start()
    {
        if(PlayerPrefs.GetInt("PlayerSucceeded") > 0)
        {
            m_winScreen.SetActive(true);
        }
        else 
        { 
            m_loseScreen.SetActive(true);        
        }
    }
}
