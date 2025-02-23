using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static characterBasics;

public class characterBasics : MonoBehaviour
{
    #region variaves

    [Header("combater variaves padrão")]
    public float Life;
    public float LifeMax;
    [HideInInspector]public float Speed;
    public float Mana;
    public float MaxMana;
    public float Medo;

    public ParticleSystem particulas;
    public SpriteRenderer[] render;

    [Header("Bonus Elementar")]
    public List<BonusDamed> bunusDamege = new List<BonusDamed>();
    public List<BonusDamed> bunusResistem = new List<BonusDamed>();

    [Header("Companheiros")]
    public List<ordemCombate> aliado = new List<ordemCombate>();
    public List<ordemCombate> inimigo = new List<ordemCombate>();

    [Header("Restistencias A elemnetos")]
    public List<element> Immunidades = new List<element>();
    public List<element> Vulnerabilities = new List<element>();
    public List<element> Resistances = new List<element>();


    [HideInInspector] public List<EfeitosCausados> EfeitoAtivos = new List<EfeitosCausados>();
    [HideInInspector] public List<EfeitosCausados> EfeitoAutoAplicadoAtivos = new List<EfeitosCausados>();

    [Header("recompensa")]
    public recompensas drop;

    [Header("CombateUi")]
    public TMP_Text damegeUi;

    [Header("som")]
    public AudioSource conficAudioEfecti;
    public AudioClip passo, falando, levandoDando, Morrendo;

    [Header("scripts")]
    public CombateControler combater;
    protected EnimyControler personagem;
    protected PlayerControler player;
    protected PlayerControler playerStript;
    //iniciativa de ataque
    protected atteck ataqueEscolido;
    protected ordemCombate personagemEscolido;
    [HideInInspector] public ordemCombate thispersonagme;
    protected bool defender;
    #endregion

    #region enum
    public enum element
    {
        fire,
        cold,
        fisico,
        necrotic,
        poison,
        thunder,
        psychic,
        lightning,
        darkness,
        distancia,
        nada
    }

    public enum efeitos 
    { 
        dano,
        diversos,
        critico,
        ignora,
        nada,
        acumulando
    }

    public enum tiposDiversos 
    {
        dano,
        volocidade,
        acerto,
        confuso,
        Imobilizacao,
        cura,
        nada,
        ista
    }

    public enum tipoCondicao
    {
        nada,
        confuso,
        Imobilizacao
    }

    public enum efeitoCondicao
    {
        nada,
        dano
    }
    #endregion

    #region efeitos causados
    public void efeitosSpeed()
    {
        if (ataqueEscolido != null)
        {
            if (EfeitoAtivos.Count != 0)
            {
                for (int i = 0; i < EfeitoAtivos.Count; i++)
                {
                    float randow = Random.Range(EfeitoAtivos[i].Mimdano, EfeitoAtivos[i].Maxdano);
                    switch (EfeitoAtivos[i].AtrubutoDiminuir)
                    {
                        case tiposDiversos.volocidade:
                            if (EfeitoAtivos[i].porcentagemDano > 0)
                            {
                                ataqueEscolido.speedAtteck = (EfeitoAtivos[i].porcentagemDano * ataqueEscolido.speedAtteck) / 100;
                            }
                            else
                            {
                                ataqueEscolido.speedAtteck -= randow;
                            }
                            EfeitoAtivos[i].rands--;
                            if (EfeitoAtivos[i].rands <= 0)
                            {
                                EfeitoAtivos.Remove(EfeitoAtivos[i]);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    public bool efeitoImobilizar()
    {
        if (EfeitoAtivos.Count != 0)
        {
            for (int i = 0; i < EfeitoAtivos.Count; i++)
            {
                switch (EfeitoAtivos[i].AtrubutoDiminuir)
                {
                    case tiposDiversos.Imobilizacao:
                        EfeitoAtivos[i].rands--;
                        if (EfeitoAtivos[i].rands <= 0)
                        {
                            EfeitoAtivos.Remove(EfeitoAtivos[i]);
                        }
                        return true;
                    default:
                        break;
                }
            }
            return false;
        }
        return false;
    }
    public void efeitoCunfucao()
    {
        if (EfeitoAtivos.Count != 0)
        {
            for (int i = 0; i < EfeitoAtivos.Count; i++)
            {
                switch (EfeitoAtivos[i].AtrubutoDiminuir)
                {
                    case tiposDiversos.confuso:

                        if (aliado.Count > 0)
                            personagemEscolido = aliado[Random.Range(0, aliado.Count)];
                        else
                            personagemEscolido = thispersonagme;

                        EfeitoAtivos[i].rands--;
                        if (EfeitoAtivos[i].rands <= 0)
                        {
                            EfeitoAtivos.Remove(EfeitoAtivos[i]);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

    }
    public bool efeitosaplicados()
    {
        if (EfeitoAtivos.Count != 0)
        {
            for (int i = 0; i < EfeitoAtivos.Count; i++)
            {
                if (EfeitoAtivos[i].efeito != efeitos.diversos)
                {
                    float danoefeito = 0;
                    for (int j = 0; j < EfeitoAtivos[i].multiplos; j++)
                    {
                        danoefeito += Random.Range(EfeitoAtivos[i].Mimdano, EfeitoAtivos[i].Maxdano);
                    }
                    levardano(new BonusDamed(EfeitoAtivos[i].elementoDoDano, danoefeito), null, true);
                    EfeitoAtivos[i].rands--;
                    if (EfeitoAtivos[i].rands <= 0)
                    {
                        EfeitoAtivos.Remove(EfeitoAtivos[i]);
                    }
                    if (Life <= 0)
                    {
                        combater.nestruen();
                        return false;
                    }
                }
                else
                {
                    float randow = Random.Range(EfeitoAtivos[i].Mimdano, EfeitoAtivos[i].Maxdano);
                    switch (EfeitoAtivos[i].AtrubutoDiminuir)
                    {
                        case tiposDiversos.dano:
                            if (EfeitoAtivos[i].porcentagemDano > 0)
                            {
                                ataqueEscolido.maxdamege = (EfeitoAtivos[i].porcentagemDano * ataqueEscolido.maxdamege) / 100;
                                ataqueEscolido.MimDamege = (EfeitoAtivos[i].porcentagemDano * ataqueEscolido.MimDamege) / 100;
                            }
                            else
                            {
                                ataqueEscolido.maxdamege -= randow;
                                ataqueEscolido.MimDamege -= randow;
                            }
                            ataqueEscolido.MimDamege = ataqueEscolido.MimDamege < 0 ? 0 : ataqueEscolido.MimDamege;
                            ataqueEscolido.maxdamege = ataqueEscolido.maxdamege < 0 ? 0 : ataqueEscolido.maxdamege;
                            EfeitoAtivos[i].rands--;
                            if (EfeitoAtivos[i].rands <= 0)
                            {
                                EfeitoAtivos.Remove(EfeitoAtivos[i]);
                            }
                            break;
                        case tiposDiversos.acerto:
                            if (EfeitoAtivos[i].porcentagemDano > 0)
                            {
                                ataqueEscolido.porcentagemDeAcerto = (EfeitoAtivos[i].porcentagemDano * ataqueEscolido.porcentagemDeAcerto) / 100;
                            }
                            else
                            {
                                ataqueEscolido.porcentagemDeAcerto -= randow;
                            }
                            EfeitoAtivos[i].rands--;
                            if (EfeitoAtivos[i].rands <= 0)
                            {
                                EfeitoAtivos.Remove(EfeitoAtivos[i]);
                            }
                            break;
                        default:
                            break;

                    }

                }
            }
        }
        return true;
    }

    #endregion

    #region função de combates
    public void levardano(BonusDamed atteck, EfeitosCausados efeitosaplicados = null, bool seaplicado = false, bool inginorar = false, float istaMorte = 0)
    {
      
        if (efeitosaplicados!= null)
        {
            EfeitoAtivos.Add(efeitosaplicados);
            if (EfeitoAtivos[EfeitoAtivos.Count - 1].efeito == efeitos.nada)
            {
                EfeitoAtivos.Remove(EfeitoAtivos[EfeitoAtivos.Count - 1]);
            }
            else
            {
                float randomAcertoEfeito = Random.Range(0, 100.0f);
                if (randomAcertoEfeito > efeitosaplicados.porcentagemAceto)
                    EfeitoAtivos.Remove(EfeitoAtivos[EfeitoAtivos.Count - 1]);
            }
        }
        if (!inginorar) 
        {
            if (Immunidades.IndexOf(atteck.Elemento) != -1)
            {
                atteck.Bonus = 0;
            }
            if (Resistances.IndexOf(atteck.Elemento) != -1)
            {
                atteck.Bonus /= 2;
            }
        }
        if (Vulnerabilities.IndexOf(atteck.Elemento) != -1)
        {
            atteck.Bonus *= 2;
        }

        if (!inginorar) 
        {
            for (int i = 0; i < bunusResistem.Count; i++)
            {
                if (bunusResistem[i].Elemento == atteck.Elemento)
                {
                    atteck.Bonus -= bunusResistem[i].Bonus;
                    if (atteck.Bonus <= 0)
                        atteck.Bonus = 0;
                }
            }
        }
        for(int i = 0;i < EfeitoAutoAplicadoAtivos.Count;i++)
        {
            if (EfeitoAutoAplicadoAtivos[i].AtrubutoDiminuir == tiposDiversos.dano)
            {
                if(EfeitoAutoAplicadoAtivos[i].porcentagemDano > 0)
                {
                    atteck.Bonus = (EfeitoAutoAplicadoAtivos[i].porcentagemDano * atteck.Bonus) / 100;
                }
                else
                {
                    atteck.Bonus -= Random.Range(EfeitoAutoAplicadoAtivos[i].Mimdano, EfeitoAutoAplicadoAtivos[i].Maxdano);
                }
                EfeitoAutoAplicadoAtivos[i].rands--;
                if (EfeitoAutoAplicadoAtivos[i].rands <= 0)
                {
                    EfeitoAutoAplicadoAtivos.Remove(EfeitoAutoAplicadoAtivos[i]);
                }
            }
        }

        if (defender)
        {
            atteck.Bonus /= 2;
        }

        Life -= atteck.Bonus;

        particulas.Play();
        conficAudioEfecti.PlayOneShot(levandoDando);

        for(int i = 0; i < render.Length; i++)
        {
            render[i].color = Color.red;
        }

        if (istaMorte != 0)
        {
            if((LifeMax * istaMorte)/100 == Life)
            {
                Life = -10;
                atteck.Bonus += 9999;
            }
        }

        if (damegeUi != null)
        {
            
            damegeUi.text += atteck.Bonus.ToString() + "\n";
            if (player != null)
                Invoke("LipesaTMP", 1f);
            else
            {
                playerStript.playertime(personagem);

            }
        }
        //Debug.Log(Life + "  " + atteck.Bonus);


        if (!seaplicado)
            if (Life <= 0)
            {
                if (damegeUi != null) 
                {
                    damegeUi.text = "";
                }
                else
                {
                    combater.playerControler = player;
                }
                dead();
            }
    }
    EnimyControler controlenimyUi;

    public void playertime(EnimyControler eni)
    {
        Invoke("time", 1f);
        controlenimyUi = eni;
    }

    void time()
    {
        controlenimyUi.damegeUi.text = "";
        for (int i = 0; i < controlenimyUi.render.Length; i++)
        {
            controlenimyUi.render[i].color = Color.white;
        }

    }

    void LipesaTMP()
    {
        damegeUi.text = "";
        for (int i = 0; i < render.Length; i++)
        {
            render[i].color = Color.white;
        }
    }
    public void dead()
    {
        conficAudioEfecti.PlayOneShot(Morrendo);

        if (player == null)
        {
            if (aliado.Count == 0)
            {
                int rec = combater.recompensaCombate.FindIndex(0, combater.recompensaCombate.Count, new recompensas(drop.Iteam, 0, Color.red).StartsWith);
                if (rec != -1)
                {
                    combater.recompensaCombate[rec].quantidadeIteam++;

                    combater.endCombater();
                    Destroy(personagem.gameObject, 0.1f);
                    return;
                }
                combater.recompensaCombate.Add(drop);

                combater.endCombater();
                
                Destroy(personagem.gameObject, 0.1f);
                return;
            }
            else
            {
                
                int a = combater.ordemCombates.FindIndex(0, combater.ordemCombates.Count, thispersonagme.StartsWith);
                if (a != -1)
                {
                    if (aliado.FindIndex(0, aliado.Count, new ordemCombate(null, playerStript).StartsWith) == -1) 
                    {
                        int rec = combater.recompensaCombate.FindIndex(0, combater.recompensaCombate.Count, new recompensas(drop.Iteam, 0, Color.red).StartsWith);
                        if (rec != -1)
                        {
                            combater.recompensaCombate[rec].quantidadeIteam++;

                            combater.ordemCombates.Remove(combater.ordemCombates[a]);
                            combater.personagmeScrips.Remove(personagem);
                            Destroy(personagem.gameObject);
                            return;
                        }
                        
                        combater.recompensaCombate.Add(drop);
                    }
                    combater.ordemCombates.Remove(combater.ordemCombates[a]);
                }
                combater.personagmeScrips.Remove(personagem);
                Destroy(personagem.gameObject);
            }
        }
        else
        {
            int a = combater.ordemCombates.FindIndex(0, combater.ordemCombates.Count, thispersonagme.StartsWith);
            if (a != -1)
                combater.ordemCombates.Remove(combater.ordemCombates[a]);

            combater.endCombater();
            //Destroy(player.gameObject, 0.1f);
        }
      
        for (int i = 0; i < aliado.Count;i++)
        {
            if (aliado[i].personagmeScrips != null || aliado[i].playerControler != null) return;
        }
        combater.endCombater();

    }

    #endregion

    #region Inimigos
    public void atualizarInimigo(List<ordemCombate> combate)
    {
        inimigo.Clear();

        for(int i = 0;i < combate.Count; i++)
        {
            if (aliado.FindIndex(0, aliado.Count, combate[i].StartsWith) == -1)
            {
                inimigo.Add(combate[i]);
            }
        }
    }
    #endregion
}

#region Tipos criados
[System.Serializable]
public class BonusDamed
{
    public element Elemento;
    public float Bonus;
    public BonusDamed(element Element, float bonus)
    {
        this.Elemento = Element;
        this.Bonus = bonus;
    }
}

[System.Serializable]
public class EfeitosCausados 
{
    [Header("tipo do efeito")]
    public efeitos efeito;
    public tiposDiversos AtrubutoDiminuir;
    [Header("pocentagem de acerto")]
    public float porcentagemAceto;
    [Header("elemento do efeito")]
    public element elementoDoDano;
    [Header("fator multiplicador do efeito")]
    public float multiplos = 1;
    [Header("Dano do efeito")]
    public float Maxdano;
    public float Mimdano;
    [Header("ativa a função de pocentagem\nquando for maior que 0")]
    public float porcentagemDano;
    [Header("tempo de duração do efeito")]
    public int rands;
    
    public EfeitosCausados(efeitos efeito, element elementoDoDano, float maxdano, float mimdano, int rands, float multipl , tiposDiversos atrubutoDiminuir,float porcentagemDano,float porcentagemAceto)
    {
        this.efeito = efeito;
        this.elementoDoDano = elementoDoDano;
        this.Maxdano = maxdano;
        this.Mimdano = mimdano;
        this.rands = rands;
        this.multiplos = multipl;
        this.AtrubutoDiminuir = atrubutoDiminuir;
        this.porcentagemDano = porcentagemDano;
        this.porcentagemAceto = porcentagemAceto;
    }

    public bool StartsWith(EfeitosCausados e)
    {
        if (e.efeito == efeito && e.AtrubutoDiminuir == AtrubutoDiminuir && e.porcentagemAceto == porcentagemAceto && e.elementoDoDano == elementoDoDano && e.multiplos == multiplos && e.Maxdano == Maxdano && e.Mimdano == Mimdano && e.porcentagemDano == porcentagemDano && e.rands == rands)
            return true;
        else
            return false;
    }
}

[System.Serializable]
public class atteck
{
    [Header("Nome")]
    public string Nome;
    [Header("\n")]
    [Header("Nome")]
    public Color cor;
    [Header("\n")]
    [Header("som")]
    public AudioClip acertando;
    public AudioClip errando;

    [Header("\n")]
    [Header("Elemento Do Ataque")]
    public element Elemento;
    [Header("\n")]
    [Header("dano do ataque")]
    public float MimDamege;
    public float maxdamege;
    [Header("\n")]
    [Header("estatistica do ataque")]
    public float porcentagemDeAcerto;
    public float speedAtteck;
    [Header("\n")]
    [Header("EfeitoAutoAplicado")]
    public EfeitosCausados efeitosAuto;
    [Header("\n")]
    [Header("condição")]
    public efeitoCondicao tipoDaCondição;
    public tiposDiversos efeitoCondicao;
    public float porcentagemCondicao;
    public float Maxcondicao;
    public float mimCondicao;
    [Header("\n")]
    [Header("efeitos")]
    public EfeitosCausados efeitos;
    [Header("\n")]
    [Header("custo Do Ataque")]
    public float custoLife = 0;
    public float custoMana = 0;



    public atteck(element Elemento,
                  float maxdamege,
                  float MimDamege,
                  float bonus,
                  EfeitosCausados efeitos,
                  float custoLife,
                  float custoMna,
                  float speedAtteck,
                  string nome,
                  tiposDiversos tipoDaCondição,
                  efeitoCondicao efeitoCondicao,
                  float porcentagemCondicao,
                  float Maxcondicao,
                  float mimCondicao,
                  EfeitosCausados efeitosAuto,
                  Color cor,
                  AudioClip acertando,
                  AudioClip errando)
    {
        this.Elemento = Elemento;
        this.MimDamege = MimDamege;
        this.maxdamege = maxdamege;
        this.porcentagemDeAcerto = bonus;
        this.efeitos = efeitos;
        this.custoLife = custoLife;
        this.custoMana = custoMna;
        this.speedAtteck = speedAtteck;
        this.Nome = nome;
        this.efeitoCondicao = tipoDaCondição;
        this.tipoDaCondição = efeitoCondicao;
        this.porcentagemCondicao = porcentagemCondicao;
        this.Maxcondicao = Maxcondicao;
        this.mimCondicao = mimCondicao;
        this.efeitosAuto = efeitosAuto;
        this.cor = cor;
        this.acertando = acertando;
        this.errando = errando;
    }
}

#endregion
