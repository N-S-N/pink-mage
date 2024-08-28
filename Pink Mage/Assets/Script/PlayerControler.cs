using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControler : characterBasics
{
    #region variavesis
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
    [SerializeField] Image vidaImagem;
    [SerializeField] Image manaImage;
    [SerializeField] TMP_Text vidaText, manaText;

    [Header("combate")]
    [SerializeField] Button ButomDoAttteck;
    [SerializeField] Image[] CorDoAtteck;
    [SerializeField] TMP_Text[] CustoDoAtteck;

    [Header("Scripts")]
    [SerializeField] Inventario inventario;

    // variaves privadas
    private Animator InimeAnimator;
    float stateTIme;
    Vector2 moveDirection = Vector2.zero;
    Rigidbody rig;
    List<Save> words = new List<Save>();
    private float TimeGame;
    private int slot;

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
        //InimeAnimator.SetInteger("State", (int)PlayerState);
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
        moveDirection = context.ReadValue<Vector2>();
    }

    public void AtivarInventario(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(!pause.activeInHierarchy)
                Inventario.SetActive(!Inventario.activeInHierarchy);
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
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
            words.Add(new Save(words.Count, 1, null, 0));
        }
        //sistema de muda~ça de valores
        words[slot].Tempo = TimeGame;
        //Scene scene = SceneManager.GetActiveScene();
        words[slot].fase = SceneManager.GetActiveScene().buildIndex;

        savePersonagem();
        //save
        saveMundo();
        SceneManager.LoadSceneAsync(0);
    }


    #endregion

    #region funciom de save
    public void saveMundo()
    {
        SaveData data = new SaveData();

        for (int i = 0; i < words.Count; i++)
        {
            Save itemdata = new Save(words[i].sloat, words[i].fase, words[i].personagem, words[i].Tempo);
            data.slotData.Add(itemdata);
        }

        string jsonData = JsonUtility.ToJson(data);

        File.WriteAllText("menu.json", jsonData);
    }
    public void savePersonagem()
    {
        personagem itemdata = new personagem(LifeMax,Life,MaxMana,Mana, Medo, inventario.la, inventario.couro);

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
                LifeMax = lineMapdafe.VidaMax;
                Life = lineMapdafe.VidaMim;
                MaxMana = lineMapdafe.ManaMax;
                Mana = lineMapdafe.ManaMim;
                Medo = lineMapdafe.medo;
                inventario.la = lineMapdafe.la;
                inventario.couro = lineMapdafe.couro;
            }
            if (File.Exists("InventarioQuadados" + slot.ToString() + ".json"))
            {
                string jsonData = File.ReadAllText("InventarioQuadados" + slot.ToString() + ".json");
                rquipamentosData lineMapdafe = JsonUtility.FromJson<rquipamentosData>(jsonData);

                for (int i = 0; i < inventario.equipamentosQuadado.Count; i++)
                {
                    TMP_Text textoimagem = inventario.equipamentosQuadado[i].texto;
                    if(lineMapdafe.slotData[i].Iteam.Nome != "")
                         inventario.equipamentosQuadado[i].texto.text = lineMapdafe.slotData[i].Iteam.Nome;

                    inventario.equipamentosQuadado = lineMapdafe.slotData;
                    inventario.equipamentosQuadado[i].texto = textoimagem;
                   
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
                    inventario.equipamentosUnsando[i] = lineMapdafe.slotData[i];
                    inventario.equipamentosUnsando[i].texto = textoimagem;
                   
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
        Vector3 move = new Vector3(moveDirection.x, rig.velocity.y, moveDirection.y) * _velocityWalk;
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
            }
            else
            {
                buttom[i].enabled = true;
                ButomDoAttteck.enabled = true;
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
        if (Inventario.activeInHierarchy || pause.activeInHierarchy)
        {
            Time.timeScale = 0;
            return true;
        }
        else if(!Inventario.activeInHierarchy && !pause.activeInHierarchy)
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


[System.Serializable]
public class personagem
{
    public float VidaMax;
    public float VidaMim;
    public float ManaMax;
    public float ManaMim;
    public float medo;
    public int la;
    public int couro;

    public personagem(float VidaMax,float VidaMim, float ManaMax, float ManaMim,float medo, int la, int couro)
    {
        this.VidaMax = VidaMax;
        this.VidaMim = VidaMim;
        this.ManaMax = ManaMax;
        this.ManaMim = ManaMim;
        this.medo = medo;
        this.la = la;
        this.couro = couro;

    }
}

public class rquipamentosData 
{
    public List<rquipamentos> slotData = new List<rquipamentos>();

}






