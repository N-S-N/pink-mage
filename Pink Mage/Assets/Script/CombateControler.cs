using System;
using System.Collections.Generic;
using UnityEngine;

public class CombateControler : MonoBehaviour
{
    [HideInInspector] public List<EnimyControler> personagmeScrips = new List<EnimyControler>();
    [HideInInspector] public PlayerControler playerControler;
    List<float> Speed;
    [HideInInspector] public List<ordemCombate> ordemCombates = new List<ordemCombate>();
    int indexrand = 0;
    [HideInInspector]public bool IsCombater = false;

    public void ordem()
    {
        IsCombater = true;

        int maior = 0;

        for (int i = 0;i < personagmeScrips.Count;i++)
        {
            Speed.Add(personagmeScrips[i].Speed);
            if(i != 0)
            {
                if (personagmeScrips[maior].Speed <= personagmeScrips[i].Speed)
                    maior = i;
            }

        }
        Speed.Add(playerControler.Speed);
        

        if (personagmeScrips[maior].Speed <= playerControler.Speed)
            maior = -1;


        if (maior == -1)
        {
            ordemCombates.Add(new ordemCombate(null, playerControler));
        }
        else
        {
            ordemCombates.Add(new ordemCombate(personagmeScrips[maior], null));
        }

        if(personagmeScrips.Count < 1)
        {
            if (maior == -1)
            {
                ordemCombates.Add(new ordemCombate(personagmeScrips[maior], null));            
            }
            else
            {
                ordemCombates.Add(new ordemCombate(null, playerControler));
            }
        }

        for (int i = 0; i < personagmeScrips.Count; i++)
        {
            if (maior == -1)
            {
                int indexnest = Speed.IndexOf(playerControler.Speed - 1);
                if(indexnest != -1)
                    ordemCombates.Add(new ordemCombate(personagmeScrips[indexnest], null));

            }
            else
            {
                int indexnest = Speed.IndexOf(personagmeScrips[maior].Speed-1);
                if (indexnest != -1)
                {
                    if (indexnest !> personagmeScrips.Count) 
                    {
                        ordemCombates.Add(new ordemCombate(personagmeScrips[indexnest], null));
                    }
                    else
                    {
                        ordemCombates.Add(new ordemCombate(null, playerControler));
                    }
                }
            }
        }

        for (int i = 0; i < ordemCombates.Count; i++)
        {
            if (i+1 < personagmeScrips.Count)
                personagmeScrips[i].atualizarInimigo(ordemCombates);
            else
                playerControler.atualizarInimigo(ordemCombates);
        }

        if (ordemCombates[indexrand].playerControler != null)
        {
            ordemCombates[indexrand].playerControler.startCombate();
        }
        else
        {
            ordemCombates[indexrand].personagmeScrips.startCombate();
        }
    }

    public void endAction()
    {
        if (ordemCombates.Count == 0) return;

        indexrand++;

        if (indexrand > ordemCombates.Count)
            indexrand = 0;

        if (ordemCombates[indexrand].playerControler != null)
        {
            ordemCombates[indexrand].playerControler.startCombate();
        }
        else
        {
            ordemCombates[indexrand].personagmeScrips.startCombate();
        }

    }

    public void endCombater()
    {
        IsCombater = false;
        personagmeScrips.Clear();
        playerControler.aliado.Clear();
        playerControler = null;
        ordemCombates.Clear();
        Speed.Clear();
        indexrand = 0;
    }
}

[System.Serializable]
public class ordemCombate
{
    [HideInInspector] public EnimyControler personagmeScrips;
    [HideInInspector] public PlayerControler playerControler;
    public ordemCombate(EnimyControler enimy, PlayerControler player)
    {
        this.personagmeScrips = enimy;
        this.playerControler = player;
    }
}