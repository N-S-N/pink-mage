using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tpspript : MonoBehaviour
{
    [SerializeField] int sala;
    [SerializeField] Vector3 posicaoDeTP;
    [SerializeField] PlayerControler playerControler;

    private void OnTriggerEnter(Collider other)
    {

        // save
        PlayerPrefs.SetInt("voltacombate", 0);
        PlayerPrefs.SetInt("Carregar", 1);

        playerControler.savePersonagem(posicaoDeTP);
        playerControler.saveMundo();

        SceneManager.LoadSceneAsync(sala);
    }

}
