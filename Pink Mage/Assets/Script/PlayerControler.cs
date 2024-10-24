using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControler : characterBasics
{
    #region variavesis

    #region variaves publics
    [SerializeField] CombateControler combate;

    [Header("compate")]
    [HideInInspector]public List<atteck> atteck = new List<atteck>();

    //movimento player
    [Header("movimentação")]
    [SerializeField] float _velocityWalk;
    [SerializeField] float _velocityRun;
    [SerializeField] Camera _camera;

    [Header("OBJ")]
    [SerializeField] GameObject Inventario;
    [SerializeField] GameObject pause;
    [SerializeField] GameObject Combater;
    [SerializeField] Button[] buttom;

    [Header("UI")]
    [SerializeField] GameObject visualizension;
    [SerializeField]public GameObject Canvas;
    [SerializeField] Image vidaImagem;
    [SerializeField] Image manaImage;
    [SerializeField] TMP_Text vidaText, manaText;

    [Header("combate")]
    [SerializeField] Button ButomDoAttteck;
    [SerializeField] GameObject defend;
    [SerializeField] Image[] CorDoAtteck;
    [SerializeField] TMP_Text[] CustoDoAtteck;
    [SerializeField] EventSystem[] events;
    [SerializeField] bool CombaterSena = false;

    [Header("Scripts")]
    [SerializeField]public Inventario inventario;

    [Header("Interacao")]
    public GameObject UiLoja;
    [HideInInspector] public Vector3 positiommSave;
    #endregion

    #region variaves privadas
    // variaves privadas

    private Animator InimeAnimator;
    float stateTIme;
    Vector2 moveDirection = Vector2.zero;
    Rigidbody rig;
    [HideInInspector] public List<Save> words = new List<Save>();
    [HideInInspector] public float TimeGame;
    [HideInInspector]public int slot;

    #endregion

    #endregion

    #region state
    enum State
    {
        Iddle,
        Running

    }
    [Header("estaddo")]
    [SerializeField] State PlayerState;


    #endregion

    #region updete e start

    private void Awake()
    {  
        slot = PlayerPrefs.GetInt("espaçoDeAmazenamento");
        TimeGame = PlayerPrefs.GetFloat("Tempo");
        loud();
    }

    private void Start()
    {

        PlayerState = State.Iddle;
        InimeAnimator = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();

        combater = combate;
        player = this;
    }

    private void Update()
    {
        //contagem do tempo
        TimeGame += Time.deltaTime;

        if (combater.IsCombater)
        {
            visualizension.SetActive(true);
            updadeStatus();
            return;
        }
        else
        {
            visualizension.SetActive(false);
        }

        if (DetectUIOpem())return;   

        float delta = Time.deltaTime;
        homdleenemyFSM(delta);
        InimeAnimator.SetFloat("state", (int)PlayerState);
    }
    #endregion

    #region FSM
    //controle de state
    void homdleenemyFSM(float deltaTIme)
    {
        //tempo de estado
        stateTIme += deltaTIme;

        //verificar as condições de troca de estado
        var newState = TryChangeCurrentState(PlayerState, stateTIme);

        //saber se trocando de estado
        if (newState != PlayerState)
        {
            //troque de estado
            OnStateExit(PlayerState);

            //Trocar pora um novo estado
            PlayerState = newState;
            stateTIme = 0;

            //Entra no novo estado
            OnStateEnter(PlayerState);
        }
        //dar uipdaite para um estado atual
        OnStateUpdete(PlayerState, deltaTIme);

    }
    /// <summary>
    /// tentar de trocar o estado atual. caso não tenha condioção de troca , vai retornar o mesmo stado que entrou
    /// </summary>
    /// <param name="State">Estado atual</param>
    /// <param name="time">tempo do estado</param>
    /// <returns>Novo estado</returns>        
    State TryChangeCurrentState(State State, float time)
    {
        switch (State)
        {
            case State.Iddle:
                //tentanbo ir para o stado correndo
                if (moveDirection != Vector2.zero)
                {
                    return State.Running;
                }
                break;
            case State.Running:
                //tentanbo ir para o stado correndo
                if (moveDirection == Vector2.zero)
                {
                    return State.Iddle;
                }
                break;
            default:
                break;

        }
        return State;
    }
    //sair do estado atual
    void OnStateExit(State State)
    {
    }
    //entra no estado atual
    void OnStateEnter(State State)
    {
    }
    //Updete do estado atual
    void OnStateUpdete(State State, float deltaTIme)
    {
        switch (State)
        {
            case State.Running:
                Movimentacoa();
                break;
            case State.Iddle:
                rig.velocity = new Vector3 (0, rig.velocity.y, 0);
                break;
            default:
                break;
        }
    }
    #endregion

    #region buttomChek
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (CombaterSena) return;
        moveDirection = context.ReadValue<Vector2>();
    }

    public void AtivarInventario(InputAction.CallbackContext context)
    {
        if (CombaterSena) return;
        if (context.performed)
        {
            if(!pause.activeInHierarchy)
                Inventario.SetActive(!Inventario.activeInHierarchy);
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (CombaterSena) return;
        if (combater.IsCombater) return;
        if (context.performed)
        {
            if (Inventario.activeInHierarchy)
            {
                Inventario.SetActive(false);
            }
            else if (pause.activeInHierarchy)
            {
                pause.SetActive(false);
            }
            else if (UiLoja.activeInHierarchy)
            {
                UiLoja.SetActive(false);
            }
            else
            {
                pause.SetActive(true);
            }
        }
    }

    public void sairPause()
    {
        pause.SetActive(false);
    }

    public void voltarMenu()
    {
        if(words.Count <= 0)
        {
            words.Add(new Save(words.Count, 1, Color.white, Color.white, Color.white, Color.white, 0));
        }
        //sistema de muda~ça de valores
        words[slot].Tempo = TimeGame;
        //Scene scene = SceneManager.GetActiveScene();
        words[slot].fase = SceneManager.GetActiveScene().buildIndex;

        savePersonagem(transform.position);
        //save
        saveMundo();
        SceneManager.LoadSceneAsync(0);
    }


    #endregion

    #region funciom de save
    public void saveMundo(bool  mapa = true)
    {
        SaveData data = new SaveData();
        if (mapa)
        {
            words[slot].personagemCapacete = inventario.equipamentosUnsando[0].Imagem;
            words[slot].personagemCapa = inventario.equipamentosUnsando[1].Imagem;
            words[slot].personagemLuva = inventario.equipamentosUnsando[2].Imagem;
            words[slot].personagemBota = inventario.equipamentosUnsando[3].Imagem;
        }
        for (int i = 0; i < words.Count; i++)
        {
            Save itemdata = new Save(words[i].sloat, words[i].fase, words[i].personagemCapacete, words[i].personagemCapa, words[i].personagemLuva, words[i].personagemBota, words[i].Tempo);
            data.slotData.Add(itemdata);
        }

        string jsonData = JsonUtility.ToJson(data);

        File.WriteAllText("menu.json", jsonData);
    }
    public void savePersonagem(Vector3 positiom)
    {
        personagem itemdata = new personagem(positiom, LifeMax,Life,MaxMana,Mana, Medo, inventario.la, inventario.couro, inventario.coracao_Envenenado, inventario.cranio, inventario.lingua);

        string jsonData = JsonUtility.ToJson(itemdata);

        File.WriteAllText("Personagem" + slot.ToString() + ".json", jsonData);

        //savar inventario
        
        rquipamentosData data = new rquipamentosData();
        rquipamentosData data2 = new rquipamentosData();
        for (int i = 0; i < inventario.equipamentosUnsando.Count; i++)
        {
            rquipamentos itemdata2 = new rquipamentos(inventario.equipamentosUnsando[i].Iteam, inventario.equipamentosUnsando[i].Imagem, inventario.equipamentosUnsando[i].texto, inventario.equipamentosUnsando[i].ImagemLocal);
            data.slotData.Add(itemdata2);
        }
        for (int i = 0; i < inventario.equipamentosQuadado.Count; i++)
        {
            rquipamentos itemdata3 = new rquipamentos(inventario.equipamentosQuadado[i].Iteam, inventario.equipamentosQuadado[i].Imagem, inventario.equipamentosQuadado[i].texto, inventario.equipamentosQuadado[i].ImagemLocal);
            data2.slotData.Add(itemdata3);
        }

        string jsonDataInvantario = JsonUtility.ToJson(data);
        string jsonDataInvantario2 = JsonUtility.ToJson(data2);
        File.WriteAllText("InventarioQuadados" + slot.ToString() + ".json", jsonDataInvantario2);
        File.WriteAllText("InventarioUsando" + slot.ToString() + ".json", jsonDataInvantario);
    }
    void loud()
    {
        //Puxar o save
        if (File.Exists("menu.json"))
        {
            string jsonData = File.ReadAllText("menu.json");

            SaveData lineMapdafe = JsonUtility.FromJson<SaveData>(jsonData);
            words = lineMapdafe.slotData;
        }

        //ver se alguma informação ser puxadas
        if (PlayerPrefs.GetInt("Carregar") == 1)
        {
            if (File.Exists("Personagem" + slot.ToString() + ".json"))
            {
                string jsonData = File.ReadAllText("Personagem" + slot.ToString() + ".json");
                personagem lineMapdafe = JsonUtility.FromJson<personagem>(jsonData);
                if (!CombaterSena)
                {
                    transform.position = lineMapdafe.positiom;
                    positiommSave = lineMapdafe.positiom;
                }
                else
                {
                    positiommSave = lineMapdafe.positiom;
                }
                LifeMax = lineMapdafe.VidaMax;
                Life = lineMapdafe.VidaMim;
                MaxMana = lineMapdafe.ManaMax;
                Mana = lineMapdafe.ManaMim;
                Medo = lineMapdafe.medo;
                inventario.la = lineMapdafe.la;
                inventario.couro = lineMapdafe.couro;
                inventario.cranio = lineMapdafe.Cranio;
                inventario.lingua = lineMapdafe.Lingua;
                inventario.coracao_Envenenado = lineMapdafe.Coracao;

            }
            
            if (File.Exists("InventarioQuadados" + slot.ToString() + ".json"))
            {
                string jsonData = File.ReadAllText("InventarioQuadados" + slot.ToString() + ".json");
                rquipamentosData lineMapdafe = JsonUtility.FromJson<rquipamentosData>(jsonData);

                for (int i = 0; i < inventario.equipamentosQuadado.Count; i++)
                {
                    TMP_Text textoimagem = inventario.equipamentosQuadado[i].texto;

                    if (lineMapdafe.slotData[i].Iteam.Nome != "")
                    {
                        inventario.equipamentosQuadado[i].texto.text = lineMapdafe.slotData[i].Iteam.Nome;

                        rquipamentos back = new rquipamentos(inventario.equipamentosQuadado[i].Iteam, inventario.equipamentosQuadado[i].Imagem, inventario.equipamentosQuadado[i].texto, inventario.equipamentosQuadado[i].ImagemLocal);
                        inventario.equipamentosQuadado[i] = lineMapdafe.slotData[i];
                        inventario.equipamentosQuadado[i].ImagemLocal = back.ImagemLocal;

                        inventario.equipamentosQuadado[i].texto = textoimagem;

                        inventario.equipamentosQuadado = lineMapdafe.slotData;
                        inventario.equipamentosQuadado[i].ImagemLocal.color = inventario.equipamentosQuadado[i].Imagem;
                        inventario.equipamentosQuadado[i].texto = textoimagem;

                    }
                }
            }
            if (File.Exists("InventarioUsando" + slot.ToString() + ".json"))
            {
                string jsonData = File.ReadAllText("InventarioUsando" + slot.ToString() + ".json");
                rquipamentosData lineMapdafe = JsonUtility.FromJson<rquipamentosData>(jsonData);

                for (int i = 0; i < inventario.equipamentosUnsando.Count; i++)
                {
                    TMP_Text textoimagem = inventario.equipamentosUnsando[i].texto;
                    inventario.equipamentosUnsando[i].texto.text = lineMapdafe.slotData[i].Iteam.Nome;

                    rquipamentos back = new rquipamentos(inventario.equipamentosUnsando[i].Iteam, inventario.equipamentosUnsando[i].Imagem, inventario.equipamentosUnsando[i].texto, inventario.equipamentosUnsando[i].ImagemLocal);
                    inventario.equipamentosUnsando[i] = lineMapdafe.slotData[i];
                    inventario.equipamentosUnsando[i].ImagemLocal = back.ImagemLocal;

                    inventario.equipamentosUnsando[i].texto = textoimagem;
                    inventario.NomeAtteck[i].text = inventario.equipamentosUnsando[i].Iteam.atteck.Nome;
                    atteck.Add(inventario.equipamentosUnsando[i].Iteam.atteck);

                }
                for (int i = 0; i < inventario.roupaPersonagem.Count; i++)
                {
                    inventario. roupaPersonagem[i].color = inventario.equipamentosUnsando[i].Imagem;
                    inventario.equipamentosUnsando[i].ImagemLocal.color = inventario.equipamentosUnsando[i].Imagem;
                    inventario.referencias[i].color = inventario.equipamentosUnsando[i].Imagem;
                }
            }
        }
        else
        {

        }
    }


    #endregion

    #region funcion de movimentação
    void Movimentacoa()
    {
        Vector3 move = new Vector3(moveDirection.x, 0, moveDirection.y) * _velocityWalk;
        move.y = rig.velocity.y;
        InimeAnimator.SetFloat("x", moveDirection.x);
        InimeAnimator.SetFloat("y", moveDirection.y);
        rig.velocity = move;
    }


    #endregion

    #region combate
    bool isImobilizedi;
    public void ButomActive()
    {
        if (efeitoImobilizar())
        {
            isImobilizedi = true;
            combater.startRond(atteck[0].speedAtteck, this, null);
            return;
        }

        Invoke("activeteButom",0.1f);
        ButomDoAttteck.enabled = false;
        for (int i = 0; i < buttom.Length; i++)
        {
            //cor
            CorDoAtteck[i].color = atteck[i].cor;
            //custo
            CustoDoAtteck[i].text = atteck[i].custoMana.ToString();

            //ativar
            if (Mana < atteck[i].custoMana)
            {
                buttom[i].enabled = false;

                //mudando o eventSystem
                if (events[1].currentSelectedGameObject == buttom[i].gameObject || events[1].currentSelectedGameObject == null)
                {
                    for (int j = i; j < buttom.Length; j++)
                    {
                        if (Mana >= atteck[j].custoMana)
                        {
                            events[1].SetSelectedGameObject(buttom[j].gameObject);
                            break;
                        }
                    }
                    for (int j = i; j >= 0; j--)
                    {
                        if (events[1].currentSelectedGameObject != null)
                        {
                            break;
                        }
                        if (Mana >= atteck[j].custoMana)
                        {
                            events[1].SetSelectedGameObject(buttom[j].gameObject);
                            break;
                        }
                    }
                }

            }
            else
            {
                buttom[i].enabled = true;
                ButomDoAttteck.enabled = true;
            }
        }

        if(ButomDoAttteck.enabled == false)
        {
            if (events[0].currentSelectedGameObject == ButomDoAttteck.gameObject || events[0].currentSelectedGameObject == null)
            {
                events[0].SetSelectedGameObject(defend);
            }

        }

        personagemEscolido = inimigo[0];
    }

    void activeteButom()
    {
        Combater.SetActive(true);
    }

    public void startCombate()
    {
        int criticobonus = 1;
        bool inginorar = false;
        float acumuladodano = 1;
        float instamorte = 0;
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

        if (ataqueEscolido == null)
        {

        }
        else
        {
            

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
                
                if(ataqueEscolido.efeitos.efeito == efeitos.critico)
                {
                    if (Random.Range(0, 100.0f) <= ataqueEscolido.efeitos.porcentagemAceto)
                    {
                        criticobonus = 2;
                    }
                }
                if (ataqueEscolido.efeitos.efeito == efeitos.ignora)
                {
                    if (Random.Range(0, 100.0f) <= ataqueEscolido.efeitos.porcentagemAceto)
                    {
                        inginorar = true;
                    }
                }
                if (ataqueEscolido.efeitos.AtrubutoDiminuir == tiposDiversos.ista)
                {
                    instamorte = ataqueEscolido.efeitos.porcentagemDano;
                }

                for (int i = 0; i < EfeitoAutoAplicadoAtivos.Count; i++)
                {
                    if (EfeitoAutoAplicadoAtivos[i].efeito == efeitos.acumulando)
                    {
                        acumuladodano = EfeitoAutoAplicadoAtivos[i].porcentagemDano;
                        EfeitoAutoAplicadoAtivos[i].porcentagemDano += EfeitoAutoAplicadoAtivos[i].porcentagemDano;
                    }
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
                                                                     (((Random.Range(ataqueEscolido.MimDamege, ataqueEscolido.maxdamege) + demegerBonus)* criticobonus) * (acumuladodano*100))/100),
                                                                     new EfeitosCausados(ataqueEscolido.efeitos.efeito,
                                                                                         ataqueEscolido.efeitos.elementoDoDano,
                                                                                         ataqueEscolido.efeitos.Maxdano,
                                                                                         ataqueEscolido.efeitos.Mimdano,
                                                                                         ataqueEscolido.efeitos.rands,
                                                                                         Random.Range(1, ataqueEscolido.efeitos.multiplos),
                                                                                         ataqueEscolido.efeitos.AtrubutoDiminuir,
                                                                                         ataqueEscolido.efeitos.porcentagemDano,
                                                                                         ataqueEscolido.efeitos.porcentagemAceto),false, inginorar, instamorte);


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
                                                                         (((Random.Range(ataqueEscolido.MimDamege, ataqueEscolido.maxdamege) + demegerBonus ) * criticobonus) * (acumuladodano * 100)) / 100),
                                                                         new EfeitosCausados(ataqueEscolido.efeitos.efeito,
                                                                                             ataqueEscolido.efeitos.elementoDoDano,
                                                                                             ataqueEscolido.efeitos.Maxdano,
                                                                                             ataqueEscolido.efeitos.Mimdano,
                                                                                             ataqueEscolido.efeitos.rands,
                                                                                             Random.Range(1, ataqueEscolido.efeitos.multiplos),
                                                                                             ataqueEscolido.efeitos.AtrubutoDiminuir,
                                                                                             ataqueEscolido.efeitos.porcentagemDano,
                                                                                             ataqueEscolido.efeitos.porcentagemAceto),false, inginorar, instamorte);
                }
                if (ataqueEscolido.efeitosAuto.efeito != efeitos.nada)
                {
                    if (ataqueEscolido.efeitosAuto.AtrubutoDiminuir == tiposDiversos.cura)
                    {
                        List<ordemCombate> AliadoJaCurado = new List<ordemCombate>();
                        for (int i = 0; i < ataqueEscolido.efeitosAuto.multiplos; i++)
                        {
                            if (aliado.Count < i)
                            {
                                int a = Random.Range(0, aliado.Count);
                                if (AliadoJaCurado.IndexOf(aliado[a]) != -1)
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
        }

        if (Life <= 0) dead();

        combater.nestruen();

    }

    public void ActionSpeed(int randomatteck)
    {
        if (randomatteck == -1)
        {
            defender = true;

            ataqueEscolido = null;

            efeitosSpeed();

            combater.startRond(atteck[0].speedAtteck, this);

            Combater.SetActive(false);
        }
        else
        {
            defender = false;

            ataqueEscolido = new atteck(atteck[randomatteck].Elemento, atteck[randomatteck].maxdamege, atteck[randomatteck].MimDamege, atteck[randomatteck].porcentagemDeAcerto, atteck[randomatteck].efeitos, atteck[randomatteck].custoLife, atteck[randomatteck].custoMana, atteck[randomatteck].speedAtteck, atteck[randomatteck].Nome, atteck[randomatteck].efeitoCondicao, atteck[randomatteck].tipoDaCondição, atteck[randomatteck].porcentagemCondicao, atteck[randomatteck].Maxcondicao, atteck[randomatteck].mimCondicao, atteck[randomatteck].efeitosAuto, atteck[randomatteck].cor);

            efeitosSpeed();

            combater.startRond(ataqueEscolido.speedAtteck, this);

            Combater.SetActive(false);
        }
    }

    public void tentarfugir()
    {
        if (combater.fugir())
        {
            combater.endCombater();
            combater.telaDeVitoria.gameObject.SetActive(false);
            combater.TelaDeFuga.gameObject.SetActive(true);
        }
        else
        {
            ataqueEscolido = null;

            efeitosSpeed();

            combater.startRond(atteck[0].speedAtteck, this);

            Combater.SetActive(false);
        }
    }
    #endregion

    #region Ui
    private bool DetectUIOpem()
    {
        if (Inventario.activeInHierarchy || pause.activeInHierarchy || UiLoja.activeInHierarchy)
        {
            Time.timeScale = 0;
            return true;
        }
        else if(!Inventario.activeInHierarchy && !pause.activeInHierarchy && !UiLoja.activeInHierarchy)
        {
            Time.timeScale = 1;
            return false;
        }
        else
        {
            return true;
        }
    }

    void updadeStatus()
    {
        vidaImagem.fillAmount = Life / LifeMax;
        manaImage.fillAmount = Mana / MaxMana;
        vidaText.text = Life.ToString() + "/" + LifeMax.ToString();
        manaText.text = Mana.ToString() + "/" + MaxMana.ToString();
    }

    #endregion
}

#region Creat list
[System.Serializable]
public class personagem
{
    public Vector3 positiom;
    public float VidaMax;
    public float VidaMim;
    public float ManaMax;
    public float ManaMim;
    public float medo;
    public int la;
    public int couro;
    public int Coracao;
    public int Cranio;
    public int Lingua;

    public personagem(Vector3 positiom,float VidaMax,float VidaMim, float ManaMax, float ManaMim,float medo, int la, int couro,int Coracao,int Cranio,int Lingua)
    {
        this.positiom = positiom;
        this.VidaMax = VidaMax;
        this.VidaMim = VidaMim;
        this.ManaMax = ManaMax;
        this.ManaMim = ManaMim;
        this.medo = medo;
        this.la = la;
        this.couro = couro;
        this.Coracao = Coracao;
        this.Cranio = Cranio;
        this.Lingua = Lingua;

    }
}

public class rquipamentosData 
{
    public List<rquipamentos> slotData = new List<rquipamentos>();

}
#endregion






