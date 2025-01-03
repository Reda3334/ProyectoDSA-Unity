using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        string scene = PlayerPrefs.GetString("sceneToLoad");
        SceneManager.LoadScene(scene);
    }
}
