using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using static characterBasics;


public class TrigerCombate : MonoBehaviour
{
    [SerializeField] public CombateControler controler;
    [SerializeField] public PlayerControler player;
    [SerializeField] public List<EnimyControler> NPC = new List<EnimyControler>();
    [HideInInspector]public BonusDamed damege = new BonusDamed(element.nada,0);

    void OnTriggerEnter(Collider other)
    {
        player.levardano(damege);
        controler.playerControler = player;
        controler.personagmeScrips = NPC;
        controler.trigerStartCombater();
    }
    public void saveMundo()
    {
        combaterSenaData itemdata2 = new combaterSenaData(null, null);

        for (int i = 0; i < NPC.Count; i++)
        {
            itemdata2.enimy.Add(NPC[i].gameObject);
        }
        for (int i = 0; i < player.aliado.Count; i++)
        {
            itemdata2.Aliados.Add(player.aliado[i].personagmeScrips.gameObject);
        }
        combaterSena itemdata = new combaterSena();

        itemdata.enimy = itemdata2;
        string jsonData = JsonUtility.ToJson(itemdata);

        File.WriteAllText("combater.json", jsonData);
        PlayerPrefs.SetInt("Carregar", 1);
        player.savePersonagem();
    }

}

[System.Serializable]
public class combaterSenaData
{
    public List<GameObject> enimy = new List<GameObject>();
    public List<GameObject> Aliados = new List<GameObject>();
    public combaterSenaData(List<GameObject> enimy, List<GameObject> Aliados)
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
