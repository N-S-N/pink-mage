using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrigerCombate : MonoBehaviour
{
    [SerializeField] CombateControler controler;
    [SerializeField] PlayerControler player;
    [SerializeField] List<EnimyControler> NPC = new List<EnimyControler>();

    private void OnTriggerEnter(Collider other)
    {
        controler.playerControler = player;
        controler.personagmeScrips = NPC;
        controler.trigerStartCombater();
    }
}
