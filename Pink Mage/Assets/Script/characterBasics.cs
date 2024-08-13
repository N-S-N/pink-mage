using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static characterBasics;

public class characterBasics : MonoBehaviour
{
    [Header("combater variaves padrão")]
    public float Life;
    public float LifeMax;
    public float Speed;
    public float Mana;
    public float Medo;

    public List<BonusDamed> bunusDamege = new List<BonusDamed>();
    public List<BonusDamed> bunusResistem = new List<BonusDamed>();

    public List<ordemCombate> aliado = new List<ordemCombate>();
    [HideInInspector] public List<ordemCombate> inimigo = new List<ordemCombate>();

    public List<element> Immunidades = new List<element>();
    public List<element> Vulnerabilities = new List<element>();
    public List<element> Resistances = new List<element>();

    [HideInInspector] public List<EfeitosCausados> EfeitoAtivos = new List<EfeitosCausados>();    

    [HideInInspector] public CombateControler combater;
    protected EnimyControler personagem;
    protected PlayerControler player;

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
        darkness

    }

    public enum efeitos 
    { 
        envenenado,
        imobiliza
    }

    public bool efeitosaplicados()
    {
        if (EfeitoAtivos.Count != 0)
        {
            for (int i = 0; i < EfeitoAtivos.Count; i++)
            {
                float danoefeito = 0;
                for (int j = 0;j < EfeitoAtivos[i].multiplos; j ++) 
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
                    combater.endAction();
                    return false;
                }
            }
        }
        return true;
    }

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
            combater.personagmeScrips.Remove(personagem);
            combater.ordemCombates.Remove(new ordemCombate(personagem, null));
            Destroy(personagem,0.1f);
        }
        else
        {
            combater.playerControler = null;
            combater.ordemCombates.Remove(new ordemCombate(null, player));
        }

        if (aliado.Count == 0)
        {
            combater.endCombater();
            return;
        }
        for (int i = 0; i < aliado.Count;i++)
        {
            if (aliado[i].personagmeScrips != null || aliado[i].playerControler != null) return;
        }
        combater.endCombater();

    }

    public void atualizarInimigo(List<ordemCombate> combate)
    {
        inimigo.Clear();

        for(int i = 0;i < combate.Count; i++)
        {
            if (aliado.IndexOf(combate[i]) == -1)
            {
                inimigo.Add(combate[i]);
            }
        }

    }

}

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
    public efeitos efeito;
    public element elementoDoDano;
    public float multiplos = 1;
    public float Maxdano;
    public float Mimdano;
    public int rands;
    public EfeitosCausados(efeitos efeito, element elementoDoDano, float maxdano, float mimdano, int rands, float multipl)
    {
        this.efeito = efeito;
        this.elementoDoDano = elementoDoDano;
        this.Maxdano = maxdano;
        this.Mimdano = mimdano;
        this.rands = rands;
        this.multiplos = multipl;
    }
}

[System.Serializable]
public class atteck
{
    public element Elemento;
    public float MimDamege;
    public float maxdamege;
    public float porcentagem;
    public EfeitosCausados efeitos;
    public float custoLife = 0;
    public float custoMana = 0;
    public atteck(element Elemento,float maxdamege,float MimDamege, float bonus, EfeitosCausados efeitos,float custoLife,float custoMna)
    {
        this.Elemento = Elemento;
        this.MimDamege = MimDamege;
        this.maxdamege = maxdamege;
        this.porcentagem = bonus;
        this.efeitos = efeitos;
        this.custoLife = custoLife;
        this.custoMana = custoMna;
    }
}
