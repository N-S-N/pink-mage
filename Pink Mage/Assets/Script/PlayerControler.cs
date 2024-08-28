using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerControler : characterBasics
{
    #region variavesis
    [SerializeField] CombateControler combate;

    [Header("compate")]
    [HideInInspector]public List<atteck> atteck = new List<atteck>();

    //movimento player
    [Header("movimenta��o")]
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

    // variaves privadas
    private Animator InimeAnimator;
    float stateTIme;
    Vector2 moveDirection = Vector2.zero;
    Rigidbody rig;

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

        //verificar as condi��es de troca de estado
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
    /// tentar de trocar o estado atual. caso n�o tenha condio��o de troca , vai retornar o mesmo stado que entrou
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
    #endregion

    #region funcion de movimenta��o
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
                            if (ataqueEscolido.tipoDaCondi��o == efeitoCondicao.dano)
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
                            if (ataqueEscolido.tipoDaCondi��o == efeitoCondicao.dano)
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

            ataqueEscolido = new atteck(atteck[randomatteck].Elemento, atteck[randomatteck].maxdamege, atteck[randomatteck].MimDamege, atteck[randomatteck].porcentagemDeAcerto, atteck[randomatteck].efeitos, atteck[randomatteck].custoLife, atteck[randomatteck].custoMana, atteck[randomatteck].speedAtteck, atteck[randomatteck].Nome, atteck[randomatteck].efeitoCondicao, atteck[randomatteck].tipoDaCondi��o, atteck[randomatteck].porcentagemCondicao, atteck[randomatteck].Maxcondicao, atteck[randomatteck].mimCondicao, atteck[randomatteck].efeitosAuto, atteck[randomatteck].cor);

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
