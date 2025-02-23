using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveControler : MonoBehaviour
{
    public List<Image> imagemDoPersonagem;
    public TMP_Text SlotDoButom;
    public TMP_Text TempoDeJogo;

    public float tempo;
    public int Slot;
    public List<Image> Imagem;
    public int fase;

    public void selecionou()
    {
        PlayerPrefs.SetInt("voltacombate", 0);
        PlayerPrefs.SetInt("Carregar", 1);
        PlayerPrefs.SetInt("espašoDeAmazenamento", Slot);
        PlayerPrefs.SetFloat("Tempo", tempo);
        SceneManager.LoadSceneAsync(fase);
    }
}
