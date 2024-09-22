using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnimyControler : characterBasics
{
    #region variaves
    [Header("compate")]
    public List<atteck> atteck = new List<atteck>();
    //[HideInInspector] public CombateControler combate;

    [Header("movimentação")]
    [SerializeField] float _velocityWalk;
    [SerializeField] float _velocityRun;

    [Header("personagem")]
    [SerializeField] GameObject sprite;
    [SerializeField] public List<GameObject> prefebEnimy;
    [SerializeField] public List<GameObject> prefebAliado;
    [Header("rota dos movimento")]
    [HideInInspector] public List<GameObject> rota = new List<GameObject>();
    int index = 0;

    [Header("amadilhas")]
    [SerializeField] comportamentoDeAticacao comportamentoDeAmadilha;
    [SerializeField] GameObject Amadilha;
    [SerializeField] float timeOFDrep;
    [SerializeField] float DanoMimAmadilha, DanoMaxAmadilha;
    [SerializeField] element elemento;

    [Header("Comportamento quando o player chega perto")]
    [SerializeField] comportamento comportamonto;

    [SerializeField] float Distancia;
    [SerializeField] float DistanciaOfTriugerCombater;

    public PlayerControler playerScripter;

    Rigidbody rb;

    NavMeshAgent navMeshAgent;

    float medoescondido;

    #endregion

    #region fora de combate
    public enum comportamento
    {
        fugir,
        persegir,
        inginorar
    }
    public enum comportamentoDeAticacao
    {
        nada,
        Armadilha,
        trasformacao,
    }

    bool combate = false;
    private void Start()
    {
        playerStript = playerScripter;
        medoescondido = Medo;
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        personagem = this;
        if(comportamentoDeAmadilha == comportamentoDeAticacao.Armadilha)
            InvokeRepeating("amadilha", timeOFDrep, timeOFDrep);
    }

    bool invokecansasado = false;
    private void Update()
    {
        //sempre fica para a camera
        sprite.transform.forward = Camera.main.transform.forward;

        if (combate) return;
        if (combater.IsCombater)
        {
            CancelInvoke();
            invokecansasado = true;
            return;
        }
        else if (invokecansasado)
        {
            invokecansasado = false;
            if (comportamentoDeAmadilha == comportamentoDeAticacao.Armadilha)
                InvokeRepeating("amadilha", timeOFDrep, timeOFDrep);
        }

        if (playerScripter == null)
        {
            navMeshAgent.speed = _velocityWalk;
            ceguindoRota();
            return;
        }

        //por distancia
        if (Vector3.Distance(playerScripter.transform.position, transform.position) < DistanciaOfTriugerCombater && Medo <= playerScripter.Medo)
        {
            if(comportamentoDeAmadilha == comportamentoDeAticacao.trasformacao)
                playerScripter.levardano (new BonusDamed(elemento, Random.Range(DanoMimAmadilha, DanoMaxAmadilha)));
            CombaterComander();
            combate = true;
        }

        if (Vector3.Distance(playerScripter.transform.position, transform.position) < Distancia && Medo <= playerScripter.Medo)
        {
            navMeshAgent.speed = _velocityRun;
            persegindo();
        }
        else
        {
            navMeshAgent.speed = _velocityWalk;
            ceguindoRota();
        }
    }
    void amadilha()
    {
        GameObject amadilha = Instantiate(Amadilha, transform.position, transform.rotation);
        TrigerCombate trig = amadilha.GetComponent<TrigerCombate>();
        trig.controler = combater;
        trig.player = playerScripter;
        trig.NPC.Add(this); 
        trig.damege = new BonusDamed(elemento, Random.Range(DanoMimAmadilha, DanoMaxAmadilha));
        Destroy(amadilha, timeOFDrep);

    }
    void persegindo()
    {
        if (comportamonto == comportamento.persegir)
        {
            navMeshAgent.SetDestination(playerScripter.transform.position);
        }
        else if (comportamonto == comportamento.fugir)
        {
            Vector3 direction = (transform.position - playerScripter.transform.position).normalized;
            Vector3 posicaoPersonagme = (direction*10)+ transform.position;

            navMeshAgent.SetDestination(posicaoPersonagme);
        }
        else
        {
            navMeshAgent.speed = _velocityWalk;
            ceguindoRota();
        }
    }

    void ceguindoRota()
    {
        if (rota == null) return;
        if (rota.Count == 0) return;

        navMeshAgent.SetDestination(rota[index].transform.position);

        if (Vector3.Distance(transform.position, rota[index].transform.position) <= 0.5f)
        {
            index++;
            if (index >= rota.Count)
            {
                index = 0;
            }
        }
    }

    [SerializeField] List<int> referencias = new List<int>();
    [SerializeField] List<int> referencias1 = new List<int>();
    public void CombaterComander()
    {
        combaterSenaData itemdata2 = new combaterSenaData(referencias, referencias1);
        
        itemdata2.enimy.Add(prefebEnimy.IndexOf(gameObject));
        
        for (int i = 0; i < playerScripter.aliado.Count; i++)
        {
            Debug.Log(prefebAliado.IndexOf(playerScripter.aliado[i].personagmeScrips.gameObject));

            itemdata2.Aliados.Add(prefebAliado.IndexOf(playerScripter.aliado[i].personagmeScrips.gameObject));
        }
        combaterSena itemdata = new combaterSena();

        itemdata.enimy = itemdata2;
        string jsonData = JsonUtility.ToJson(itemdata);

        File.WriteAllText("combater.json", jsonData);
        PlayerPrefs.SetInt("Carregar", 1);
        PlayerPrefs.SetInt("espaçoDeAmazenamento", playerScripter.slot);
        playerScripter.savePersonagem();
        playerScripter.saveMundo();
        playerScripter.voltarMenu();
        SceneManager.LoadScene("Combater");
    }
    #endregion

    #region combater

    bool isImobilizedi;
    public void startCombate()
    {
        if (!combater.IsCombater) return;

        isImobilizedi = false;
        if (efeitoImobilizar())
        {
            isImobilizedi = true;
            combater.startRond(atteck[0].speedAtteck, null, this);
            return;
        }

        int randominimigo = Random.Range(0, inimigo.Count);
        personagemEscolido = inimigo[randominimigo];

        List<int> anterior = new List<int>();

        int randomatteck = 0;
        if (inimigo[randominimigo].playerControler != null)
        {
            for (int i = 0; i < atteck.Count; i++)
            {
                randomatteck = Random.Range(0, atteck.Count);
                anterior.Add(randomatteck);
                if (inimigo[randominimigo].playerControler.Immunidades.IndexOf(atteck[randomatteck].Elemento) != -1
                    || atteck[randomatteck].custoMana > Mana
                    || atteck[randomatteck].custoLife >= Life)
                {            
                    if (anterior.IndexOf(randomatteck) != 1 || atteck[randomatteck].custoMana > Mana || atteck[randomatteck].custoLife >= Life)
                    {
                        i--;
                    }
                    else if (atteck[randomatteck].custoMana > Mana)
                    {
                        i--;
                    }
                    else if (atteck[randomatteck].custoLife >= Life && anterior.Count - 1 < atteck.Count)
                    {
                        i--;
                    }
                    anterior.Add(randomatteck);
                }
                else
                    break;     

            }
        }
        else
        {         
            for (int i = 0; i < atteck.Count; i++)
            {
                randomatteck = Random.Range(0, atteck.Count - 1);
                anterior.Add(randomatteck);
                if (inimigo[randominimigo].personagmeScrips.Immunidades.IndexOf(atteck[randomatteck].Elemento) != -1
                    || atteck[randomatteck].custoMana > Mana
                    || atteck[randomatteck].custoLife >= Life)
                {
                    if (anterior.IndexOf(randomatteck) != 1 || atteck[randomatteck].custoMana > Mana || atteck[randomatteck].custoLife >= Life)
                    {
                        i--;
                    }
                    else if (atteck[randomatteck].custoMana > Mana)
                    {
                        i--;
                    }
                    else if (atteck[randomatteck].custoLife >= Life && anterior.Count - 1 < atteck.Count)
                    {
                        i--;
                    }
                    anterior.Add(randomatteck);
                }
                else
                    break;
            }
        }

        if (!combater.IsCombater) return;
            ataqueEscolido = new atteck(atteck[randomatteck].Elemento, atteck[randomatteck].maxdamege, atteck[randomatteck].MimDamege, atteck[randomatteck].porcentagemDeAcerto, atteck[randomatteck].efeitos, atteck[randomatteck].custoLife, atteck[randomatteck].custoMana, atteck[randomatteck].speedAtteck, atteck[randomatteck].Nome, atteck[randomatteck].efeitoCondicao, atteck[randomatteck].tipoDaCondição, atteck[randomatteck].porcentagemCondicao, atteck[randomatteck].Maxcondicao, atteck[randomatteck].mimCondicao, atteck[randomatteck].efeitosAuto, atteck[randomatteck].cor);
        if (!combater.IsCombater) return;
            efeitosSpeed();
        if (!combater.IsCombater) return;
            combater.startRond(ataqueEscolido.speedAtteck,null,this);
    }

    public void Atteck()
    {
        if (!efeitosaplicados())
        {
            dead();
            return;
        }
        if (isImobilizedi == true)
        {
            combater.nestruen();
            return;
        }
        efeitoCunfucao();


        float randomAcerto = Random.Range(0, 100.0f);
        float demegerBonus = 0;

        Mana -= ataqueEscolido.custoMana;
        Life -= ataqueEscolido.custoLife;

        if (randomAcerto <= ataqueEscolido.porcentagemDeAcerto)
        {
            for (int i = 0; i < bunusDamege.Count; i++)
            {
                if (bunusDamege[i].Elemento == ataqueEscolido.Elemento)
                    demegerBonus += bunusDamege[i].Bonus;
            }
            if (personagemEscolido.playerControler != null)
            {
                for (int i = 0; i < personagemEscolido.playerControler.EfeitoAtivos.Count; i++)
                {
                    if (personagemEscolido.playerControler.EfeitoAtivos[i].AtrubutoDiminuir == ataqueEscolido.efeitoCondicao)
                    {
                        if (ataqueEscolido.tipoDaCondição == efeitoCondicao.dano)
                        {
                            if (ataqueEscolido.porcentagemCondicao > 0)
                            {
                                demegerBonus += (ataqueEscolido.porcentagemCondicao * Random.Range(ataqueEscolido.MimDamege, ataqueEscolido.maxdamege)) / 100;
                            }
                            else
                            {
                                demegerBonus += Random.Range(ataqueEscolido.mimCondicao, ataqueEscolido.Maxcondicao);
                            }

                        }
                        break;
                    }
                }
                personagemEscolido.playerControler.levardano(new BonusDamed(ataqueEscolido.Elemento,
                                                                 Random.Range(ataqueEscolido.MimDamege, ataqueEscolido.maxdamege) + demegerBonus),
                                                                 new EfeitosCausados(ataqueEscolido.efeitos.efeito,
                                                                                     ataqueEscolido.efeitos.elementoDoDano,
                                                                                     ataqueEscolido.efeitos.Maxdano,
                                                                                     ataqueEscolido.efeitos.Mimdano,
                                                                                     ataqueEscolido.efeitos.rands,
                                                                                     Random.Range(1, ataqueEscolido.efeitos.multiplos),
                                                                                     ataqueEscolido.efeitos.AtrubutoDiminuir,
                                                                                     ataqueEscolido.efeitos.porcentagemDano,
                                                                                     ataqueEscolido.efeitos.porcentagemAceto));

            }
            else
            {
                for (int i = 0; i < personagemEscolido.personagmeScrips.EfeitoAtivos.Count; i++)
                {
                    if (personagemEscolido.personagmeScrips.EfeitoAtivos[i].AtrubutoDiminuir == ataqueEscolido.efeitoCondicao)
                    {
                        if (ataqueEscolido.tipoDaCondição == efeitoCondicao.dano)
                        {
                            if (ataqueEscolido.porcentagemCondicao > 0)
                            {
                                demegerBonus += (ataqueEscolido.porcentagemCondicao * Random.Range(ataqueEscolido.MimDamege, ataqueEscolido.maxdamege)) / 100;
                            }
                            else
                            {
                                demegerBonus += Random.Range(ataqueEscolido.mimCondicao, ataqueEscolido.Maxcondicao);
                            }

                        }
                        break;
                    }
                }
                personagemEscolido.personagmeScrips.levardano(new BonusDamed(ataqueEscolido.Elemento,
                                                                     Random.Range(ataqueEscolido.MimDamege, ataqueEscolido.maxdamege) + demegerBonus),
                                                                     new EfeitosCausados(ataqueEscolido.efeitos.efeito,
                                                                                         ataqueEscolido.efeitos.elementoDoDano,
                                                                                         ataqueEscolido.efeitos.Maxdano,
                                                                                         ataqueEscolido.efeitos.Mimdano,
                                                                                         ataqueEscolido.efeitos.rands,
                                                                                         Random.Range(1, ataqueEscolido.efeitos.multiplos),
                                                                                         ataqueEscolido.efeitos.AtrubutoDiminuir,
                                                                                         ataqueEscolido.efeitos.porcentagemDano,
                                                                                         ataqueEscolido.efeitos.porcentagemAceto));

            }
            if(ataqueEscolido.efeitosAuto.efeito != efeitos.nada)
            {
                if (ataqueEscolido.efeitosAuto.AtrubutoDiminuir == tiposDiversos.cura)
                {
                    List<ordemCombate> AliadoJaCurado = new List<ordemCombate>();
                    for (int i = 0; i < ataqueEscolido.efeitosAuto.multiplos; i++)
                    {
                        if (aliado.Count < i)
                        {
                            int a = Random.Range(0, aliado.Count);
                            if(AliadoJaCurado.IndexOf(aliado[a]) != -1)
                                AliadoJaCurado.Add(aliado[a]);
                            else
                            {
                                while (true)
                                {
                                    int b = Random.Range(0, aliado.Count);
                                    if (AliadoJaCurado.IndexOf(aliado[b]) != -1)
                                    {
                                        AliadoJaCurado.Add(aliado[b]);
                                        break;
                                    }
                                }            
                            }
                            if (AliadoJaCurado[i].playerControler == null)
                            {
                                if (ataqueEscolido.efeitosAuto.porcentagemDano <= 0)
                                    AliadoJaCurado[i].personagmeScrips.Life += Random.Range(ataqueEscolido.efeitosAuto.Mimdano, ataqueEscolido.efeitosAuto.Maxdano);
                                else
                                    AliadoJaCurado[i].personagmeScrips.Life = (AliadoJaCurado[i].personagmeScrips.Life * ataqueEscolido.efeitosAuto.porcentagemDano) / 100;
                            }
                            else
                            {
                                if (ataqueEscolido.efeitosAuto.porcentagemDano <= 0)
                                    AliadoJaCurado[i].playerControler.Life += Random.Range(ataqueEscolido.efeitosAuto.Mimdano, ataqueEscolido.efeitosAuto.Maxdano);
                                else
                                    AliadoJaCurado[i].playerControler.Life = (AliadoJaCurado[i].playerControler.Life * ataqueEscolido.efeitosAuto.porcentagemDano) / 100;
                            }
                        }
                        else
                        {
                            if (ataqueEscolido.efeitosAuto.porcentagemDano <= 0)
                                Life += Random.Range(ataqueEscolido.efeitosAuto.Mimdano, ataqueEscolido.efeitosAuto.Maxdano);
                            else
                                Life = (Life * ataqueEscolido.efeitosAuto.porcentagemDano) / 100;
                        }
                    }
                }
                else 
                {
                    float randomAcertoEfeito = Random.Range(0, 100.0f);
                    if (ataqueEscolido.efeitosAuto.porcentagemAceto <= randomAcertoEfeito)
                    {
                        EfeitoAutoAplicadoAtivos.Add(new EfeitosCausados(ataqueEscolido.efeitosAuto.efeito,
                                                                                             ataqueEscolido.efeitosAuto.elementoDoDano,
                                                                                             ataqueEscolido.efeitosAuto.Maxdano,
                                                                                             ataqueEscolido.efeitosAuto.Mimdano,
                                                                                             ataqueEscolido.efeitosAuto.rands,
                                                                                             Random.Range(1, ataqueEscolido.efeitosAuto.multiplos),
                                                                                             ataqueEscolido.efeitosAuto.AtrubutoDiminuir,
                                                                                             ataqueEscolido.efeitosAuto.porcentagemDano,
                                                                                             ataqueEscolido.efeitosAuto.porcentagemAceto));
                    }
                }
            }
        }
        if (Life <= 0) dead();

        combater.nestruen();
    }

    #endregion

    #region endCombate

    public void fimCombateFuga()
    {
        Medo = 2000;
        Invoke("contagemFuga",5f);
        inimigo.Clear();
    } 

    void contagemFuga()
    {
        Medo = medoescondido;
    }

    #endregion
}
