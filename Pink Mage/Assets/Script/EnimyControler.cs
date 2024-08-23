using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnimyControler : characterBasics
{
    #region variaves
    [Header("compate")]
    public List<atteck> atteck = new List<atteck>();
    //[HideInInspector] public CombateControler combate;

    [Header("movimentação")]
    [SerializeField] float _velocityWalk;
    [SerializeField] float _velocityRun;

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


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        personagem = this;
        if(comportamentoDeAmadilha == comportamentoDeAticacao.Armadilha)
            InvokeRepeating("amadilha", timeOFDrep, timeOFDrep);
    }

    private void Update()
    {
        if (combater.IsCombater) return;

        if (playerScripter == null)
        {
            navMeshAgent.speed = _velocityWalk;
            ceguindoRota();
            return;
        }

        //por distancia
        if (Vector3.Distance(playerScripter.transform.position, transform.position) < DistanciaOfTriugerCombater && Medo <= playerScripter.Medo)
        {
            combater.playerControler = playerScripter;
            combater.personagmeScrips.Clear();
            combater.personagmeScrips.Add(this);
            if(comportamentoDeAmadilha == comportamentoDeAticacao.trasformacao)
                playerScripter.levardano (new BonusDamed(elemento, Random.Range(DanoMimAmadilha, DanoMaxAmadilha)));
            combater.trigerStartCombater();
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

    #endregion

    #region combater

    bool isImobilizedi;
    public void startCombate()
    {
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

        ataqueEscolido = new atteck(atteck[randomatteck].Elemento, atteck[randomatteck].maxdamege, atteck[randomatteck].MimDamege, atteck[randomatteck].porcentagemDeAcerto, atteck[randomatteck].efeitos, atteck[randomatteck].custoLife, atteck[randomatteck].custoMana, atteck[randomatteck].speedAtteck, atteck[randomatteck].Nome, atteck[randomatteck].efeitoCondicao, atteck[randomatteck].tipoDaCondição, atteck[randomatteck].porcentagemCondicao, atteck[randomatteck].Maxcondicao, atteck[randomatteck].mimCondicao, atteck[randomatteck].efeitosAuto);

        efeitosSpeed();

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
}
