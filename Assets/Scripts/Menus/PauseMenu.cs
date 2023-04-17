using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    #region Menu objects to manipulate
    [SerializeField]
    private GameObject background;
    [SerializeField]
    private GameObject victoryText;
    [SerializeField]
    private GameObject continueButton;
    #endregion

    private void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Enable();
        LevelManager.instance.player.GetComponent<PlayerStatus>().winCallback += VictoryScreen;
    }

    private void OnToggleMenu()
    {
        background.SetActive(!background.activeSelf);
        if (background.activeSelf) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    private void VictoryScreen()
    {
        background.SetActive(true);
        victoryText.SetActive(true);
        continueButton.SetActive(false);
        Time.timeScale = 0;
    }

    #region Button methods
    public void ContinueGame()
    {
        Time.timeScale = 1;
        background.SetActive(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void LoadMaze()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
    #endregion
}
