using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class spwamSeneControl : MonoBehaviour
{
    [Header("configf")]
    [SerializeField] public CombateControler controler;
    [SerializeField] public PlayerControler player;
    [SerializeField] public GameObject[] spawm;
    [SerializeField] public GameObject[] spawmAliado;
    [SerializeField] UiControler Uicontroler;

    [Header("spawm")]

    [SerializeField] public List <GameObject> prefebEnimy;
    [SerializeField] public List<EnimyControler> NPC = new List<EnimyControler>();
    [HideInInspector] public List<int> intprerfebenimy;
    [SerializeField] public List<GameObject> prefebAliado;
    [SerializeField] public List<EnimyControler> NPCAniado = new List<EnimyControler>();
    [HideInInspector] public List<int> intprefebfrends;


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
            intprerfebenimy = lineMapdafe.enimy.enimy;
            intprefebfrends = lineMapdafe.enimy.Aliados;
        }
        for (int o = 0; o < Uicontroler.controler.Count; o++)
        {
            if (Uicontroler.controler[o].personagem == null)
            {
                Uicontroler.controler[o].personagem = player.transform;
                player.damegeUi = Uicontroler.controler[o].Vunabilidades.GetComponent<TMP_Text>();
                break;
            }
        }
        for (int i = 0; i < intprerfebenimy.Count; i++)
        {
            if (intprerfebenimy[i] != -1)
            {
                GameObject enimy = Instantiate(prefebEnimy[intprerfebenimy[i]], spawm[i].transform, spawm[i].transform);
                enimy.transform.localPosition = Vector3.zero;
                EnimyControler controlerEnymy = enimy.GetComponent<EnimyControler>();
                for (int o = 0; o < Uicontroler.controler.Count; o++)
                {
                    if (Uicontroler.controler[o].personagem == null)
                    {
                        Uicontroler.controler[o].personagem = enimy.transform;
                        controlerEnymy.damegeUi = Uicontroler.controler[o].Vunabilidades.GetComponent<TMP_Text>();

                        break;
                    }
                }
                controlerEnymy.combater = controler;
                controlerEnymy.rota = null;
                controlerEnymy.playerScripter = player;
                NPC.Add(controlerEnymy);
            }
        }
        for (int i = 0; i < intprefebfrends.Count; i++)
        {
            if (intprefebfrends[i] != -1)
            {
                GameObject enimy = Instantiate(prefebAliado[intprefebfrends[i]], spawmAliado[i].transform, spawmAliado[i].transform);
                enimy.transform.localPosition = Vector3.zero;
                EnimyControler controlerEnymy = enimy.GetComponent<EnimyControler>();
                for (int o = 0; o < Uicontroler.controler.Count; o++)
                {
                    if (Uicontroler.controler[o].personagem == null)
                    {
                        Uicontroler.controler[o].personagem = enimy.transform;
                        controlerEnymy.damegeUi = Uicontroler.controler[o].Vunabilidades.GetComponent<TMP_Text>();
                        break;
                    }
                }
                controlerEnymy.combater = controler;
                controlerEnymy.rota = null;
                controlerEnymy.playerScripter = player;
                NPCAniado.Add(controlerEnymy);

                player.aliado.Add(new ordemCombate(NPCAniado[i], null));
                
            }
        }

    }

    void combater()
    {
        List<EnimyControler> controlerentidades = new List<EnimyControler>();
        controlerentidades.AddRange(NPC);
        controlerentidades.AddRange(NPCAniado);
        controler.playerControler = player;
        controler.personagmeScrips = controlerentidades;
        controler.trigerStartCombater();
    }
}
