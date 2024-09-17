using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class spwamSeneControl : MonoBehaviour
{
    [Header("configf")]
    [SerializeField] public CombateControler controler;
    [SerializeField] public PlayerControler player;
    [SerializeField] public GameObject[] spawm;
    [SerializeField] public GameObject[] spawmAliado;
    [Header("spawm")]
    [SerializeField] public List <GameObject> prefebEnimy;
    [SerializeField] public List<EnimyControler> NPC = new List<EnimyControler>();
    [SerializeField] public List<GameObject> prefebAliado;
    [SerializeField] public List<EnimyControler> NPCAniado = new List<EnimyControler>();


    private void Awake()
    {
        Load();  
    }
    private void Start()
    {
        combater();
    }

    //puxar todas as informaçãos
    void Load()
    {
        NPC.Clear();
        if (File.Exists("combater.json"))
        {
            string jsonData = File.ReadAllText("combater.json");

            combaterSena lineMapdafe = JsonUtility.FromJson<combaterSena>(jsonData);
            prefebEnimy = lineMapdafe.enimy.enimy;
            prefebAliado = lineMapdafe.enimy.Aliados;
        }
        for (int i = 0; i < prefebEnimy.Count; i++)
        {
            GameObject enimy = Instantiate(prefebEnimy[i], spawm[i].transform, spawm[i].transform);

            EnimyControler controlerEnymy = enimy.GetComponent<EnimyControler>();
            controlerEnymy.combater = controler;
            controlerEnymy.rota = null;
            controlerEnymy.playerScripter = player;
            NPC.Add(controlerEnymy);
        }
        for (int i = 0; i < prefebAliado.Count; i++)
        {
            GameObject enimy = Instantiate(prefebAliado[i], spawmAliado[i].transform, spawmAliado[i].transform);
            EnimyControler controlerEnymy = enimy.GetComponent<EnimyControler>();
            controlerEnymy.combater = controler;
            controlerEnymy.rota = null;
            controlerEnymy.playerScripter = player;
            NPCAniado.Add(controlerEnymy);
            player.aliado.Add(new ordemCombate(NPCAniado[i],null));
        }
    }

    void combater()
    {
        controler.playerControler = player;
        controler.personagmeScrips = NPC;
        controler.personagmeScrips = NPCAniado;
        controler.trigerStartCombater();
    }
}
