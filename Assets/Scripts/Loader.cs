using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;

    void Awake()
    {
        if (GameManager.instance == null)
            Instantiate(gameManager);
    }

    public void CloseGame()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityWrapper = new AndroidJavaClass("com.example.proyectodsa_android.activity.UnityWrapperActivity");
        unityWrapper.CallStatic("closeActivity");
#elif UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("hola");
#endif
    }
}
