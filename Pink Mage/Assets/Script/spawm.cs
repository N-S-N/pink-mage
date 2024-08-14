using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawm : MonoBehaviour
{
    [Header("onde spawnar")]
    [SerializeField] region _regionSpam;

    [Header("rota dos movimento")]
    [SerializeField] List<GameObject> rota = new List<GameObject>();

    [Header("prefeb dos inimigos baseado no local")]
    [SerializeField] List<prebEnimySpawmn> _sapwmPrefeb = new List<prebEnimySpawmn>();

    [Header("scripts")]
    [SerializeField] CombateControler combate;
    private EnimyControler controlerEnymy;
    [SerializeField] PlayerControler player;

    public enum region
    {
        florest,
        castel,
        gate
    }

    private void Start()
    {

        //spanar um inimigo aleatorio baseado na posisão
        GameObject spawm = Instantiate(_sapwmPrefeb[(int)_regionSpam].spawn[Random.Range(0, _sapwmPrefeb[(int)_regionSpam].spawn.Count - 1)],
                                       transform.position,
                                       transform.rotation);

        controlerEnymy = spawm.GetComponent<EnimyControler>();
        controlerEnymy.combater = combate;
        controlerEnymy.rota = rota;
        controlerEnymy.playerScripter = player;
        
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