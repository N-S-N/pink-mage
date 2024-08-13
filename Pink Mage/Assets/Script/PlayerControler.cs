using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : characterBasics
{
    #region variavesis
    [SerializeField] CombateControler combate;

    //movimento player
    [Header("movimentação")]
    [SerializeField] float _velocityWalk;
    [SerializeField] float _velocityRun;
    [SerializeField] Camera _camera;


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
    

    #endregion

    #region funcion de movimentação
    void Movimentacoa()
    {
        Vector3 move = new Vector3(moveDirection.x, rig.velocity.y, moveDirection.y) * _velocityWalk;
        rig.velocity = move;
    }


    #endregion

    #region combate

    public void startCombate()
    {
        if (!efeitosaplicados()) return;

    }



    #endregion
}
