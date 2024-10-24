using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawm : MonoBehaviour
{
    [Header("onde spawnar")]
    [SerializeField] region _regionSpam;
    [SerializeField] int localspaem;

    [Header("rota dos movimento")]
    [SerializeField] List<GameObject> rota = new List<GameObject>();

    [Header("prefeb dos inimigos baseado no local")]
    [SerializeField] List<prebEnimySpawmn> _sapwmPrefeb = new List<prebEnimySpawmn>();

    [Header("scripts")]
    [SerializeField] CombateControler combate;
    [HideInInspector]public EnimyControler controlerEnymy;
    [SerializeField] PlayerControler player;
    [SerializeField] public controlerSpaner control;
    public bool issapner = false;
    public enum region
    {
        florest,
        cidade,
        castelo
    }

    private void Start()
    {
        if (issapner) return;
        //spanar um inimigo aleatorio baseado na posisão
        GameObject spawm = Instantiate(_sapwmPrefeb[(int)_regionSpam].spawn[Random.Range(0, _sapwmPrefeb[(int)_regionSpam].spawn.Count - 1)],
                                       transform.position,
                                       transform.rotation,
                                       gameObject.transform.parent);

        controlerEnymy = spawm.GetComponent<EnimyControler>();
        controlerEnymy.combater = combate;
        controlerEnymy.rota = rota;
        controlerEnymy.playerScripter = player;
        controlerEnymy.spawmnwelocal = localspaem;
        controlerEnymy.control = control;
        controlerEnymy.medoescondido = controlerEnymy.Medo;
    }

    public void spawmactivete(GameObject obj, Vector3 positiom)
    {
        GameObject spawm = Instantiate(obj,
                                       positiom,
                                       transform.rotation,
                                       gameObject.transform.parent);

        controlerEnymy = spawm.GetComponent<EnimyControler>();
        controlerEnymy.combater = combate;
        controlerEnymy.rota = rota;
        controlerEnymy.playerScripter = player;
        controlerEnymy.spawmnwelocal = localspaem;
        controlerEnymy.control = control;
        controlerEnymy.transform.position = positiom;
        controlerEnymy.medoescondido = controlerEnymy.Medo;
        controlerEnymy.fimCombateFuga();
        
        //Invoke("endmedo", 5f);
        
    }
    void endmedo()
    {
        Debug.Log("aaa");
        controlerEnymy.contagemFuga();
    } 
}


[System.Serializable]
public class prebEnimySpawmn
{
    public List<GameObject> spawn;
    public prebEnimySpawmn(List<GameObject> spawmn)
    {
        this.spawn = spawmn;
    }
}
