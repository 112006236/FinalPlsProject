using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private string sceneName;

    // Call this to load a scene by name
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    // Call this to reload the current active scene
    public void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Call this to quit the game
    public void QuitGame()
    {
        Application.Quit();

        // ONLY FOR UNITY EDITOR, DELETE THIS FOR REAL GAME
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
