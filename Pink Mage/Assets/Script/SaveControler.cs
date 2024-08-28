using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveControler : MonoBehaviour
{
    public Image imagemDoPersonagem;
    public TMP_Text SlotDoButom;
    public TMP_Text TempoDeJogo;

    public float tempo;
    public int Slot;
    public Sprite Imagem;
    public int fase;

    public void selecionou()
    {
        PlayerPrefs.SetInt("Carregar", 1);
        PlayerPrefs.SetInt("espaçoDeAmazenamento", Slot);
        PlayerPrefs.SetFloat("Tempo", tempo);
        SceneManager.LoadSceneAsync(fase);
    }
}
