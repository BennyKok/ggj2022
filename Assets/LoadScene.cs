using System.Collections;
using System.Collections.Generic;
using BennyKok.Bootstrap;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void ReloadCurrent()
    {
        // SceneManager.LoadScene(Bootstrap.Instance.gameObject.scene.name);
                
        DeathScreen.Instance.gameObject.SetActive(true);
        DeathScreen.Instance.OK(Bootstrap.Instance.gameObject);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
