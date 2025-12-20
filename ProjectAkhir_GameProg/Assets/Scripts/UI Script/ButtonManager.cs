using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Animator fadeAnimator;

    // Call this to load a scene by name
    public void LoadScene()
    {
        StartCoroutine(FadeLoadScene());
    }

    IEnumerator FadeLoadScene()
    {
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }

    // Call this to reload the current active scene
    public void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(0);
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
