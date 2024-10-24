using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
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
    [HideInInspector] public List<int> intprerfebenimyindex;
    [HideInInspector] public List<combaterSenaslot> combaterprerfebenimy;
    [SerializeField] public List<GameObject> prefebAliado;
    [SerializeField] public List<EnimyControler> NPCAniado = new List<EnimyControler>();
    [HideInInspector] public List<int> intprefebfrends;
    [HideInInspector] public List<int> intprefebfrendsindex;
    [HideInInspector] public List<combaterSenaslot> combaterprerfebfrends;

     public List<GameObject> enimy = new List<GameObject>();
     public List<GameObject> frendy = new List<GameObject>();

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

            combaterprerfebenimy = lineMapdafe.enimy.enimy;
            combaterprerfebfrends = lineMapdafe.enimy.Aliados;

            intprefebfrends.Clear();
            intprerfebenimy.Clear();
            intprefebfrendsindex.Clear();
            intprerfebenimyindex.Clear();

            for (int i = 0; i < combaterprerfebfrends.Count; i++)
            {
                if (combaterprerfebenimy[i].incombater)
                {
                    intprefebfrends.Add(combaterprerfebfrends[i].enimy);
                    intprefebfrendsindex.Add(i);
                }
            }
            for (int i = 0; i < combaterprerfebenimy.Count; i++)
            {
                if (combaterprerfebenimy[i].incombater)
                {
                    intprerfebenimy.Add(combaterprerfebenimy[i].enimy);
                    intprerfebenimyindex.Add(i);
                }
            }

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
                enimy.Add(Instantiate(prefebEnimy[intprerfebenimy[i]], spawm[i].transform, spawm[i].transform));
                enimy[i].transform.localPosition = Vector3.zero;
                EnimyControler controlerEnymy = enimy[i].GetComponent<EnimyControler>();
                for (int o = 0; o < Uicontroler.controler.Count; o++)
                {
                    if (Uicontroler.controler[o].personagem == null)
                    {
                        Uicontroler.controler[o].personagem = enimy[i].transform;
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
                frendy.Add( Instantiate(prefebAliado[intprefebfrends[i]], spawmAliado[i].transform, spawmAliado[i].transform));
                frendy[i].transform.localPosition = Vector3.zero;
                EnimyControler controlerEnymy = frendy[i].GetComponent<EnimyControler>();
                for (int o = 0; o < Uicontroler.controler.Count; o++)
                {
                    if (Uicontroler.controler[o].personagem == null)
                    {
                        Uicontroler.controler[o].personagem = frendy[i].transform;
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
