using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static characterBasics;


public class TrigerCombate : MonoBehaviour
{
    [SerializeField] public CombateControler controler;
    [SerializeField] public PlayerControler player;
    [SerializeField] public List<EnimyControler> NPC = new List<EnimyControler>();
    [HideInInspector]public BonusDamed damege = new BonusDamed(element.nada,0);
    [SerializeField] List<combaterSenaslot> referencias = new List<combaterSenaslot>();
    [SerializeField] List<combaterSenaslot> referencias1 = new List<combaterSenaslot>();
    [SerializeField] public List<GameObject> prefebEnimy;
    [SerializeField] public List<GameObject> prefebAliado;
    [SerializeField] public controlerSpaner control;

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

        for(int i = 0; i < control.spawms.Count; i++)
        {
            for (int j = 0; j < NPC.Count;j++ ) 
            {
                if (control.spawms[i].controlerEnymy.spawmnwelocal == NPC[j].spawmnwelocal)
                {
                    itemdata2.enimy.Add(new combaterSenaslot(prefebEnimy.IndexOf(control.spawms[i].controlerEnymy.gameObject), control.spawms[i].controlerEnymy.transform.position, control.spawms[i].controlerEnymy.spawmnwelocal, true));
                    break;
                }
                else if (j+1 == NPC.Count)
                {
                    itemdata2.enimy.Add(new combaterSenaslot(prefebEnimy.IndexOf(control.spawms[i].controlerEnymy.gameObject), control.spawms[i].controlerEnymy.transform.position, control.spawms[i].controlerEnymy.spawmnwelocal, false));
                }
            }
        }
        for (int i = 0; i < player.aliado.Count; i++)
        {
            itemdata2.Aliados.Add(new combaterSenaslot(prefebAliado.IndexOf(player.aliado[i].personagmeScrips.gameObject), player.aliado[i].personagmeScrips.transform.position));
        }
        combaterSena itemdata = new combaterSena();

        itemdata.enimy = itemdata2;
        string jsonData = JsonUtility.ToJson(itemdata);

        File.WriteAllText("combater.json", jsonData);
        PlayerPrefs.SetInt("Carregar", 1);
        PlayerPrefs.SetInt("espaçoDeAmazenamento", player.slot);
        player.savePersonagem(player.transform.position);
        player.saveMundo();
        player.voltarMenu();
        SceneManager.LoadScene("Combater");
    }

}

[System.Serializable]
public class combaterSenaData
{
    public List<combaterSenaslot> enimy = new List<combaterSenaslot>();
    public List<combaterSenaslot> Aliados = new List<combaterSenaslot>();
    public combaterSenaData(List<combaterSenaslot> enimy, List<combaterSenaslot> Aliados)
    {
        this.enimy = enimy;
        this.Aliados = Aliados;
    }
}
[System.Serializable]
public class combaterSenaslot 
{
    public int enimy;
    public int spwnar;
    public bool incombater;
    public Vector3 positiom;
    public int lifeincombater;
    public combaterSenaslot(int enimy, Vector3 positiom , int spwnar = -1, bool incombater = false, int lifeincombater = -1)
    {
        this.positiom = positiom;
        this.enimy = enimy;
        this.spwnar = spwnar;
        this.incombater = incombater;
        this.lifeincombater = lifeincombater;
    }
}


[System.Serializable]
public class combaterSena
{
    public combaterSenaData enimy;
}
