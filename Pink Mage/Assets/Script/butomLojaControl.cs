using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class butomLojaControl : MonoBehaviour
{
    public ButtomLoja controler;

}



#region criat a class
[System.Serializable]
public class ButtomLoja
{
    [Header("Iteam")]
    public Image FundoDeIndisponibilidade;
    public TMP_Text Nome;
    public Image Imagem;
    [Header("how much")]
    public List<RecsitoLoja> bay;

    public ButtomLoja(Image FundoDeIndisponibilidade, TMP_Text Nome, Image imagem, List<RecsitoLoja> bay)
    {
        this.FundoDeIndisponibilidade = FundoDeIndisponibilidade;
        this.Nome = Nome;
        this.Imagem = imagem;
        this.bay = bay;
    }
}

[System.Serializable]
public class RecsitoLoja
{
    public Image imagem;
    public TMP_Text quantidade;

    public RecsitoLoja(Image imagem, TMP_Text quantidade)
    {
        this.imagem = imagem;
        this.quantidade = quantidade;
    }
}
#endregion