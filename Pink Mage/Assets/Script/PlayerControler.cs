using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    }

    private void Update()
    {
        if(DetectUIOpem())return;
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
            Inventario.SetActive(!Inventario.activeInHierarchy);
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

    public void ButomActive()
    {

    }

    public void startCombate()
    {
        if (!efeitosaplicados())
        {
            dead();
            return;
        }

        float randomAcerto = Random.Range(0, 100.0f);
        float demegerBonus = 0;

        Mana -= ataqueEscolido.custoMana;
        Life -= ataqueEscolido.custoLife;

        if (randomAcerto <= ataqueEscolido.porcentagemDeAcerto)
        {
            for (int i = 0; i < bunusDamege.Count; i++)
            {
                if (bunusDamege[i].Elemento == ataqueEscolido.Elemento)
                    demegerBonus += bunusResistem[i].Bonus;
            }
            if (personagemEscolido.playerControler != null)
                personagemEscolido.playerControler.levardano(new BonusDamed(ataqueEscolido.Elemento,
                                                                 Random.Range(ataqueEscolido.MimDamege, ataqueEscolido.maxdamege) + demegerBonus),
                                                                 new EfeitosCausados(ataqueEscolido.efeitos.efeito,
                                                                                     ataqueEscolido.efeitos.elementoDoDano,
                                                                                     ataqueEscolido.efeitos.Maxdano,
                                                                                     ataqueEscolido.efeitos.Mimdano,
                                                                                     ataqueEscolido.efeitos.rands,
                                                                                     Random.Range(1, ataqueEscolido.efeitos.multiplos),
                                                                                     ataqueEscolido.efeitos.AtrubutoDiminuir,
                                                                                     ataqueEscolido.efeitos.porcentagemDano));
            else
                personagemEscolido.personagmeScrips.levardano(new BonusDamed(ataqueEscolido.Elemento,
                                                                     Random.Range(ataqueEscolido.MimDamege, ataqueEscolido.maxdamege) + demegerBonus),
                                                                     new EfeitosCausados(ataqueEscolido.efeitos.efeito,
                                                                                         ataqueEscolido.efeitos.elementoDoDano,
                                                                                         ataqueEscolido.efeitos.Maxdano,
                                                                                         ataqueEscolido.efeitos.Mimdano,
                                                                                         ataqueEscolido.efeitos.rands,
                                                                                         Random.Range(1, ataqueEscolido.efeitos.multiplos),
                                                                                         ataqueEscolido.efeitos.AtrubutoDiminuir,
                                                                                         ataqueEscolido.efeitos.porcentagemDano));

        }

        if (Life <= 0) dead();

        combater.nestruen();

    }

    public void ActionSpeed(int randomatteck)
    {
        ataqueEscolido = new atteck(atteck[randomatteck].Elemento, atteck[randomatteck].maxdamege, atteck[randomatteck].MimDamege, atteck[randomatteck].porcentagemDeAcerto, atteck[randomatteck].efeitos, atteck[randomatteck].custoLife, atteck[randomatteck].custoMana, atteck[randomatteck].speedAtteck);

        efeitosSpeed();

        combater.startRond(ataqueEscolido.speedAtteck, this);
    }

    #endregion

    #region Ui
    private bool DetectUIOpem()
    {
        if (Inventario.activeInHierarchy)
        {
            Time.timeScale = 0;
            return true;
        }
        else
        {
            Time.timeScale = 1;
            return false;
        }
    }

    #endregion
}
