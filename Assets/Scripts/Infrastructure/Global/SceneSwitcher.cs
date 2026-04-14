using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    #region Public Methods

    public void ToScene(int id)
    {
        SceneManager.LoadScene(id);
    }

    public void QuitGame() => Application.Quit();

    #endregion
}