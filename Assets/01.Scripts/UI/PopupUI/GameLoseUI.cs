using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoseUI : UIBase
{
    [SerializeField] Button lobbyButton;

    private void Reset()
    {
        lobbyButton = GetComponentInChildren<Button>();
    }

    protected override void Start()
    {
        UIManager.Instance.AddUI(this);
        gameObject.SetActive(false);
        lobbyButton.onClick.AddListener(GoToLobby);
    }

    void GoToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

}
