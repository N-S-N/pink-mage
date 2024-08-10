using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnimyControler : characterBasics
{
    [Header("compate")]
    public List<atteck> atteck = new List<atteck>();
    //[HideInInspector] public CombateControler combate;

    [Header("movimentação")]
    [SerializeField] float _velocityWalk;
    [SerializeField] float _velocityRun;

    [Header("rota dos movimento")]
    [HideInInspector] public List<GameObject> rota = new List<GameObject>();
    int index = 0;

    [Header("Comportamento quando o player chega perto")]
    [SerializeField] comportamento comportamonto;
    [SerializeField] float Distancia;
    [HideInInspector] public PlayerControler playerScripter;

    Rigidbody rb;

    public enum comportamento
    {
        fugir,
        persegir,
        inginorar
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (combater.IsCombater) return;

        if (Vector3.Distance(playerScripter.transform.position , transform.position) < Distancia)
            persegindo();
        else
            ceguindoRota();     
    }

    void persegindo()
    {
        if (comportamonto == comportamento.persegir)
        {
            Vector3 direction = (playerScripter.transform.position - transform.position).normalized;
            direction *= _velocityRun;
            direction.y = rb.velocity.y;

            rb.velocity = direction;
        }
        else if (comportamonto == comportamento.fugir)
        {
            Vector3 direction = (transform.position - playerScripter.transform.position).normalized;
            direction *= _velocityRun;
            direction.y = rb.velocity.y;

            rb.velocity = direction;
        }
        else
        {
            ceguindoRota();
        }
    }

    void ceguindoRota()
    {
        if (rota.Count == 0) return;

        Vector3 novaVelocidade = rota[index].transform.position - transform.position;
        novaVelocidade.Normalize();
        novaVelocidade *= _velocityWalk;
        novaVelocidade.y = rb.velocity.y;

        rb.velocity = novaVelocidade;

        float distanciaDoPonto = Vector3.Distance(transform.position, rota[index].transform.position);
        if (distanciaDoPonto <= 0.5f)
        {
            index++;
            if (index >= rota.Count)
            {
                index = 0;
            }
        }
    }

    public void startCombate()
    {
        int randominimigo = Random.Range(0, inimigo.Count-1);

        int randomatteck = Random.Range(0, atteck.Count-1);

        List<int> anterior = new List<int>();
        anterior.Add(randomatteck);

        if (inimigo[randominimigo].playerControler != null)
        {
            for (int i = 0; i < atteck.Count; i++)
            {
                if (inimigo[randominimigo].playerControler.Immunidades.IndexOf(atteck[randomatteck].atributos.Elemento) != -1)
                {
                    int randomatteck2 = Random.Range(0, atteck.Count - 1);
                    if (anterior.IndexOf(randomatteck2) != 1)
                    {
                        i--;
                    }
                    anterior.Add(randomatteck2);
                }
                else
                    break;
            }
            //ver se acertou
            float randomAcerto = Random.Range(0, 100.0f);
            if (randomAcerto <= atteck[randomatteck].porcentagem)
            {
                for (int i = 0; i < bunusDamege.Count; i++)
                {
                    if (bunusDamege[i].Elemento == atteck[randomatteck].atributos.Elemento)
                        atteck[randomatteck].atributos.Bonus += bunusResistem[i].Bonus;
                }
                inimigo[randominimigo].playerControler.levardano(atteck[randomatteck].atributos);
            }
        }
        else
        {         
            for (int i = 0; i < atteck.Count; i++)
            {
                if (inimigo[randominimigo].personagmeScrips.Immunidades.IndexOf(atteck[randomatteck].atributos.Elemento) != -1)
                {
                    int randomatteck2 = Random.Range(0, atteck.Count - 1);
                    if (anterior.IndexOf(randomatteck2) != 1)
                    {
                        i--;
                    }
                    anterior.Add(randomatteck2);
                }
                else
                    break;
            }
            //ver se acertou
            float randomAcerto = Random.Range(0, 100.0f);
            if (randomAcerto <= atteck[randomatteck].porcentagem)
            {
                for (int i = 0; i < bunusDamege.Count; i++)
                {
                    if (bunusDamege[i].Elemento == atteck[randomatteck].atributos.Elemento)
                        atteck[randomatteck].atributos.Bonus += bunusResistem[i].Bonus;
                    
                }
                inimigo[randominimigo].personagmeScrips.levardano(atteck[randomatteck].atributos);
            }
        }

        combater.endAction();
    }
}
