using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlerMenu : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("Carregar", 0);
        SceneManager.LoadSceneAsync(1);
    }
    
    public void LoadGame(int espa�o)
    {
        PlayerPrefs.SetInt("Carregar", 1);
        PlayerPrefs.SetInt("espa�oDeAmazenamento", espa�o);
        SceneManager.LoadSceneAsync(1);
    }
}
