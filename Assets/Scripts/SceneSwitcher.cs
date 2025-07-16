using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string sceneName;
    public string nextSceneName;

    public void SwitchScene()
    {
        SceneManager.LoadScene(sceneName);
    }
    public void SwitchToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
