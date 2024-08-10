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
    public List<BonusDamed> bunusDamege = new List<BonusDamed>();
    public List<BonusDamed> bunusResistem = new List<BonusDamed>();

    public List<ordemCombate> aliado = new List<ordemCombate>();
    [HideInInspector] public List<ordemCombate> inimigo = new List<ordemCombate>();

    public List<element> Immunidades = new List<element>();
    public List<element> Vulnerabilities = new List<element>();
    public List<element> Resistances = new List<element>();

    [HideInInspector] public CombateControler combater;
    protected EnimyControler personagem;
    protected PlayerControler player;

    public enum element
    {
        fire,
        cold,
        bludgeoning,
        slashing,
        piercing,
        necrotic,
        poison,
        thunder,
        psychic,
        lightning,
        darkness

    }
    public void levardano(BonusDamed atteck)
    {
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
    public void aticlizarInimigo(List<ordemCombate> combate)
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
public class atteck
{
    public BonusDamed atributos;
    public float porcentagem;
    public atteck(BonusDamed Elemento, float bonus)
    {
        this.atributos = Elemento;
        this.porcentagem = bonus;
    }
}
