using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartUI : UIBase
{
    [SerializeField] Button startButton;

    private void Reset()
    {
        startButton = GetComponentInChildren<Button>();
    }

    protected override void Start()
    {
        UIManager.Instance.AddUI(this);
        startButton.onClick.AddListener(GameStart);
    }

    void GameStart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
