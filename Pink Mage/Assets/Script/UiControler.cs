using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiControler : MonoBehaviour
{
    public List<ControlerUi> controler = new List<ControlerUi>();

    void FixedUpdate()
    {
        for (int i = 0; i < controler.Count; i ++) 
        {
            if (controler[i].personagem != null) 
            {
                Vector2 positiom = Camera.main.WorldToScreenPoint(controler[i].personagem.position);
                controler[i].Selectiom.position = positiom;
                controler[i].Vunabilidades.position = positiom;
                controler[i].Demege.position = positiom;

            }
        }
    }
}

[System.Serializable]
public class ControlerUi
{
    public Transform personagem;
    public RectTransform Selectiom;
    public RectTransform Vunabilidades;
    public RectTransform Demege;

    public ControlerUi(Transform personagem, RectTransform Selectiom, RectTransform Vunabilidades, RectTransform Demege)
    {
        this.personagem = personagem;
        this.Selectiom = Selectiom;
        this.Vunabilidades = Vunabilidades;
        this.Demege = Demege;
    }
}
