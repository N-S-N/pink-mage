using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        indexCauntPersonagem ++ ;
        Speed.Add((float)speed);
        ordemCombates2.Add(new ordemCombate(personagem, player));

        if (indexCauntPersonagem == ordemCombates.Count)
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
        Speed.Clear();
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