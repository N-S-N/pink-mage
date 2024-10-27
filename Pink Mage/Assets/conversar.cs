using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class conversar : MonoBehaviour
{
    [SerializeField] List<string> fala = new List<string>();

    [SerializeField] UnityEvent evento;

    [SerializeField] TMP_Text text;

    [SerializeField] GameObject UI;


    int index = 0;

    public bool ativado = false;



    private void Start()
    {
        if (text == null) 
        {
            UI.SetActive(false);
            return;
        }
        text.text = "...";
    }

    private void Update()
    {
        if(UI.activeInHierarchy)
            UI.transform.transform.forward = Camera.main.transform.forward;
    }

    public void dialogo()
    {
        
        if (text == null)
        {
            evento.Invoke();
            return;
        }
        if (index >= fala.Count)
        {
            evento.Invoke();
            return;
        }

        if (text.text == "...")
        {
            text.text = fala[index];
        }
        else
        {      
            text.text = fala[index];
        }

        index++;

        if (index >= fala.Count)
        {
            ativado = true;
        }
        evento.Invoke();
    }

    public void sairInterective()
    {
        ativado = false;
    }
}


