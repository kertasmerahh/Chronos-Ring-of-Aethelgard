using UnityEngine;
using UnityEngine.SceneManagement;

public class PindahScene : MonoBehaviour
{
    public void pindahScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }

    public void PlayGame()
    {
        pindahScene();
    }

    public void exitGame()
    {
        Application.Quit();
    }
}

