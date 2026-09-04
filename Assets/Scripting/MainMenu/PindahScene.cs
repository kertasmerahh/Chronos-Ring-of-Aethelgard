using UnityEngine;
using UnityEngine.SceneManagement;
public class PindahScene : MonoBehaviour
{
    public void pindahScene()
    {
        SceneManager.LoadScene("Gameplay");
    }
    public void exitGame()
    {
        Application.Quit();
    }
}
