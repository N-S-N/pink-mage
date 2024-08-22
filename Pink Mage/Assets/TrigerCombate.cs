using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static characterBasics;


public class TrigerCombate : MonoBehaviour
{
    [SerializeField] public CombateControler controler;
    [SerializeField] public PlayerControler player;
    [SerializeField] public List<EnimyControler> NPC = new List<EnimyControler>();
    [HideInInspector]public BonusDamed damege = new BonusDamed(element.nada,0);

    void OnTriggerEnter(Collider other)
    {
        player.levardano(damege);
        controler.playerControler = player;
        controler.personagmeScrips = NPC;
        controler.trigerStartCombater();
    }
}
