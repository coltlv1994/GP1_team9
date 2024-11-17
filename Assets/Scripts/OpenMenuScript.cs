using UnityEngine;

public class OpenMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject m_inGameMenu;
    [SerializeField] private bool m_menuOpened = false;
    void OnOpenMenu()
    {
        m_menuOpened = !m_menuOpened;

        m_inGameMenu.SetActive(m_menuOpened);
        m_inGameMenu.GetComponent<InGameMenuScript>().PauseGame(m_menuOpened);
    }

    public void SetOpenMenu()
    {
        m_menuOpened = !m_menuOpened;

        m_inGameMenu.SetActive(m_menuOpened);
        m_inGameMenu.GetComponent<InGameMenuScript>().PauseGame(m_menuOpened);
    }

    public bool IsMenuOpened()
    {
        return m_menuOpened;
    }
}
