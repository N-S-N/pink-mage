using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static characterBasics;


public class TrigerCombate : MonoBehaviour
{
    [SerializeField] public CombateControler controler;
    [SerializeField] public PlayerControler player;
    [SerializeField] public List<EnimyControler> NPC = new List<EnimyControler>();
    [HideInInspector]public BonusDamed damege = new BonusDamed(element.nada,0);
    [SerializeField] List<int> referencias = new List<int>();
    [SerializeField] List<int> referencias1 = new List<int>();
    [SerializeField] public List<GameObject> prefebEnimy;
    [SerializeField] public List<GameObject> prefebAliado;

    void OnTriggerEnter(Collider other)
    {
        player.levardano(damege);
        controler.playerControler = player;
        controler.personagmeScrips = NPC;
        controler.trigerStartCombater();
    }
    public void saveMundo()
    {
        combaterSenaData itemdata2 = new combaterSenaData(referencias, referencias1);

        for (int i = 0; i < NPC.Count; i++)
        {
            itemdata2.enimy.Add(prefebEnimy.IndexOf(NPC[i].gameObject));
        }
        for (int i = 0; i < player.aliado.Count; i++)
        {
            itemdata2.Aliados.Add(prefebAliado.IndexOf(player.aliado[i].personagmeScrips.gameObject));
        }
        combaterSena itemdata = new combaterSena();

        itemdata.enimy = itemdata2;
        string jsonData = JsonUtility.ToJson(itemdata);

        File.WriteAllText("combater.json", jsonData);
        PlayerPrefs.SetInt("Carregar", 1);
        PlayerPrefs.SetInt("espaçoDeAmazenamento", player.slot);
        player.savePersonagem();
        player.saveMundo();
        player.voltarMenu();
        SceneManager.LoadScene("Combater");
    }

}

[System.Serializable]
public class combaterSenaData
{
    public List<int> enimy = new List<int>();
    public List<int> Aliados = new List<int>();
    public combaterSenaData(List<int> enimy, List<int> Aliados)
    {
        this.enimy = enimy;
        this.Aliados = Aliados;
    }
}


[System.Serializable]
public class combaterSena
{
    public combaterSenaData enimy;
}
