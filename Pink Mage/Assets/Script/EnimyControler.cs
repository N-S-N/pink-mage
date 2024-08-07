using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnimyControler : characterBasics
{
    [Header("compate")]
    public List<atteck> atteck = new List<atteck>();
    [SerializeField] CombateControler combate;

    private void Start()
    {
        combater = combate;
    }

    public void startCombate()
    {
        int randominimigo = Random.Range(0, inimigo.Count);

        int randomatteck = Random.Range(0, atteck.Count);

        List<int> anterior = new List<int>();
        anterior.Add(randomatteck);

        if (inimigo[randominimigo].playerControler != null)
        {
            for (int i = 0; i < atteck.Count; i++)
            {
                if (inimigo[randominimigo].playerControler.Immunidades.IndexOf(atteck[randomatteck].atributos.Elemento) != -1)
                {
                    int randomatteck2 = Random.Range(0, atteck.Count);
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
                    int randomatteck2 = Random.Range(0, atteck.Count);
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
