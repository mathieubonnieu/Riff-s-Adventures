using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.Collections;

public class BtnOnClick : MonoBehaviour
{
    [Tooltip("Name of the scene to load when the button is clicked.")]
    public SceneAsset scene;

    public Overlay overlay;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(LoadScene);
        }
        else
        {
            Debug.LogWarning("SceneLoaderButton script must be attached to a UI Button.");
        }
    }

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(scene.name))
        {
            StartCoroutine(LoadSceneAfterFade());
        }
        else
        {
            Debug.LogWarning("Scene name is not set in SceneLoaderButton.");
        }
    }

    private IEnumerator LoadSceneAfterFade()
    {
        overlay.startFade();

        while (!overlay.isComplete())
        {
            yield return null;
        }

        SceneManager.LoadScene(scene.name);
    }

}
