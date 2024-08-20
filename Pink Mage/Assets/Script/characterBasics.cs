using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    public float Medo;

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

    public CombateControler combater;
    protected EnimyControler personagem;
    protected PlayerControler player;

    //iniciativa de ataque
    protected atteck ataqueEscolido;
    protected ordemCombate personagemEscolido;
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

    }

    public enum efeitos 
    { 
        dano,
        imobiliza
    }

    public enum tiposDiversos 
    {
        dano,
        volocidade,
        acerto,

    }

    #endregion

    #region efeitos causados
    public void efeitosSpeed()
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
                if (EfeitoAtivos[i].efeito != efeitos.imobiliza)
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
    public void levardano(BonusDamed atteck, EfeitosCausados efeitosaplicados = null, bool seaplicado = false)
    {
        if(efeitosaplicados!= null)
        {
            EfeitoAtivos.Add(efeitosaplicados);
        }

        if (Immunidades.IndexOf(atteck.Elemento) != -1)
        {
            atteck.Bonus = 0;
        }
        if (Resistances.IndexOf(atteck.Elemento) != -1)
        {
            atteck.Bonus /= 2;
        }
        if (Vulnerabilities.IndexOf(atteck.Elemento) != -1)
        {
            atteck.Bonus *= 2;
        }

        for (int i = 0; i < bunusResistem.Count; i++)
        {
            if(bunusResistem[i].Elemento == atteck.Elemento)
            {
                atteck.Bonus -= bunusResistem[i].Bonus;
                if (atteck.Bonus <= 0)
                    atteck.Bonus = 0;
            }
        }

        Life -= atteck.Bonus;
        if(!seaplicado)
            if (Life <= 0)
                dead();

    }

    public void dead()
    {
        if(player == null)
        {
            if (aliado.Count == 0)
            {
                combater.endCombater();
                Destroy(personagem, 0.1f);
                return;
            }
            else
            {
                combater.personagmeScrips.Remove(personagem);
                combater.ordemCombates.Remove(combater.ordemCombates[combater.ordemCombates.FindIndex(0, combater.ordemCombates.Count, new ordemCombate(personagem, null).StartsWith)]);
                Destroy(personagem, 0.1f);
            }
        }
        else
        {
            combater.endCombater();
            combater.playerControler = null;
            combater.ordemCombates.Remove(combater.ordemCombates[combater.ordemCombates.FindIndex(0, combater.ordemCombates.Count, new ordemCombate(null, player).StartsWith)]);
            Destroy(player, 0.1f);
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
    
    public EfeitosCausados(efeitos efeito, element elementoDoDano, float maxdano, float mimdano, int rands, float multipl , tiposDiversos atrubutoDiminuir,float porcentagemDano)
    {
        this.efeito = efeito;
        this.elementoDoDano = elementoDoDano;
        this.Maxdano = maxdano;
        this.Mimdano = mimdano;
        this.rands = rands;
        this.multiplos = multipl;
        this.AtrubutoDiminuir = atrubutoDiminuir;
        this.porcentagemDano = porcentagemDano;
    }
}

[System.Serializable]
public class atteck
{
    [Header("Nome")]
    public string Nome;
    [Header("Elemento Do Ataque")]
    public element Elemento;
    [Header("dano do ataque")]
    public float MimDamege;
    public float maxdamege;
    [Header("estatistica do ataque")]
    public float porcentagemDeAcerto;
    public float speedAtteck;
    [Header("efeitos")]
    public EfeitosCausados efeitos;
    [Header("custo Do Ataque")]
    public float custoLife = 0;
    public float custoMana = 0;
    public atteck(element Elemento,float maxdamege,float MimDamege, float bonus, EfeitosCausados efeitos,float custoLife,float custoMna,float speedAtteck, string nome)
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
    }
}

#endregion
