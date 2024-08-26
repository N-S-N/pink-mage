using System.Collections.Generic;
using UnityEngine;

public class CombateControler : MonoBehaviour
{
    [HideInInspector] public List<EnimyControler> personagmeScrips = new List<EnimyControler>();
    [HideInInspector] public PlayerControler playerControler;
    public List<float> Speed;
    public List<ordemCombate> ordemCombates = new List<ordemCombate>();
    [HideInInspector] public List<ordemCombate> ordemCombates2 = new List<ordemCombate>();
    public List<ordemCombate> ordemCombates3 = new List<ordemCombate>();
    [HideInInspector]public bool IsCombater = false;
    int indexCauntPersonagem = 0;

    public void trigerStartCombater()
    {
        IsCombater = true;
        ordemCombates.Clear();
        ordemCombates.Add(new ordemCombate(null, playerControler));
        playerControler.thispersonagme = ordemCombates[0];
        for (int i = 0; i < personagmeScrips.Count; i++)
        {
            ordemCombates.Add(new ordemCombate(personagmeScrips[i], null));
            personagmeScrips[i].thispersonagme = ordemCombates[i+1];
        }
        playerControler.aliado.Add(new ordemCombate(null, playerControler));
        playerControler.atualizarInimigo(ordemCombates);
                playerControler.ButomActive();
        for (int i = 0; i < personagmeScrips.Count; i++)
        {
            personagmeScrips[i].atualizarInimigo(ordemCombates);
            personagmeScrips[i].startCombate();
        }
    }

    public void startRond(float speed, PlayerControler player = null , EnimyControler personagem = null)
    {
        if (!IsCombater) return;
        if (playerControler == null) return;
        if (ordemCombates.Count < 2) return;

        indexCauntPersonagem ++ ;
        Speed.Add((float)speed);
        ordemCombates2.Add(new ordemCombate(personagem, player));

        if (indexCauntPersonagem == ordemCombates.Count && ordemCombates.Count > 1)
        {
            for (int i = 0; i < ordemCombates2.Count; i++)
            {
                int mas = 0;
                for (int j = 0; j < ordemCombates2.Count; j++)
                {
                    if (Speed[j] > Speed[mas]) mas = j;
                }
                ordemCombates3.Add(new ordemCombate(ordemCombates2[mas].personagmeScrips, ordemCombates2[mas].playerControler));
                ordemCombates2.Remove(ordemCombates2[mas]);
            }
            ordemCombates3.Add(new ordemCombate(ordemCombates2[0].personagmeScrips, ordemCombates2[0].playerControler));

            indexCauntPersonagem = 0;
            Speed.Clear();
            ordemCombates2.Clear();

            if (ordemCombates3[0].playerControler != null)
            {
                PlayerControler playersave = ordemCombates3[0].playerControler;
                ordemCombates3.Remove(ordemCombates3[0]);

                playersave.startCombate();
            }
            else
            {
                EnimyControler enimy = ordemCombates3[0].personagmeScrips;
                ordemCombates3.Remove(ordemCombates3[0]);

                enimy.Atteck();
            }
                 
        }
    }

    public void nestruen()
    {

        if (!IsCombater) return;
        if (ordemCombates.Count == 1)
        {
            endCombater();
            return;
        }

        if(ordemCombates3.Count == 0)
        {
            playerControler.ButomActive();
            for (int i = 0; i < personagmeScrips.Count; i++)
            {
                personagmeScrips[i].startCombate();
            }
            return;
        }

        if (ordemCombates3[0].playerControler != null)
        {
            PlayerControler playersave = ordemCombates3[0].playerControler;
            ordemCombates3.Remove(ordemCombates3[0]);

            playersave.startCombate();
        }
        else
        {
            EnimyControler enimy = ordemCombates3[0].personagmeScrips;
            ordemCombates3.Remove(ordemCombates3[0]);

            enimy.Atteck();
        }
    }

    public void endCombater()
    {
        IsCombater = false;
        personagmeScrips.Clear();
        if (playerControler != null)
        {
            playerControler.aliado.Clear();
            playerControler = null;
        }
        ordemCombates.Clear();
        ordemCombates2.Clear();
        ordemCombates3.Clear();
        Speed.Clear();
        indexCauntPersonagem = 0;
    }

    public bool fugir()
    {
        float chancerplayer = Random.Range(0,20)+ playerControler.atteck[0].speedAtteck;

        for (int i = 0; i < personagmeScrips.Count; i ++)
        {
            float chancerEnimy = Random.Range(0, 20) + personagmeScrips[i].atteck[0].speedAtteck;

            if (playerControler.aliado.FindIndex(0, playerControler.aliado.Count, new ordemCombate(personagmeScrips[i], null).StartsWith) == -1)
            {
                if (chancerplayer < chancerEnimy)
                {

                    return false;
                }

            }
        }
        for (int o = 0; o < personagmeScrips.Count; o++)
        {
            personagmeScrips[o].fimCombateFuga();
            personagmeScrips[o].inimigo.Clear();
        }
        return true;
    }

}

[System.Serializable]
public class ordemCombate
{
    public EnimyControler personagmeScrips;
    public PlayerControler playerControler;
    public ordemCombate(EnimyControler enimy, PlayerControler player)
    {
        this.personagmeScrips = enimy;
        this.playerControler = player;
    }

    public bool StartsWith(ordemCombate e)
    {
        if (e.personagmeScrips == personagmeScrips && e.playerControler == playerControler)
            return true;
        else 
            return false;
    }
}