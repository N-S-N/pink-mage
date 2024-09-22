using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlerUiCombater : MonoBehaviour
{
    [Header("vitoria")]
    [SerializeField] bool vatoria;
    public TMP_Text ItemRecebidos; 
    public List<recompensas> recompensaCombate = new List<recompensas>();

    private void OnEnable()
    {
        if(vatoria)
        {
            Invoke("tame", 0.1f);
        }
    }
    void tame()
    {
        //esquever os Iteam ganahados
        ItemRecebidos.text = "";
        for (int i = 0; i < recompensaCombate.Count; i++)
        {
            ItemRecebidos.text += recompensaCombate[i].Iteam.Nome + " X " + recompensaCombate[i].quantidadeIteam.ToString() + "\n";
        }
    }
}

[System.Serializable]
public class recompensas
{
    public ItemaConfig Iteam;
    public int quantidadeIteam;
    public Color color;
    public recompensas(ItemaConfig Iteam, int quantidadeIteam, Color color)
    {
        this.quantidadeIteam = quantidadeIteam;
        this.Iteam = Iteam;
        this.color = color;
    }
    public bool StartsWith(recompensas e)
    {
        if (e.Iteam == Iteam)
            return true;
        else
            return false;
    }
}