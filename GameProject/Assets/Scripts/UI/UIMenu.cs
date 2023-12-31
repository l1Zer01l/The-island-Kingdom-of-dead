using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public static UIMenu instance { get; private set; }

    private GameManager m_gameManager;
    private PlayerMovement m_playerMovement;
    private PlayerLook m_playerLook;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }

    private void Start()
    {
        m_gameManager = GameManager.instance;
        ReferenceSystem.OnFindedObjecs += OnInitPlayer;

    }

    private void OnInitPlayer()
    {
        ReferenceSystem.OnFindedObjecs -= OnInitPlayer;

        m_playerMovement = ReferenceSystem.instance.player.GetComponent<PlayerMovement>();
        m_playerLook = ReferenceSystem.instance.player.GetComponent<PlayerLook>();
        SetVisible(false);
    }

    public void SetVisible(bool state)
    {
        m_gameManager.SetCursorVisible(state);
        gameObject.SetActive(state);
        m_playerMovement.SetMove(!state);
        if (state)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }


    public void OnContinue()
    {
        m_playerLook.ProcessLookMenu();
    }

}
