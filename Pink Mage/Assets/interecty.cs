using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class interecty : MonoBehaviour
{
    [SerializeField] List<conversar> conversars = new List<conversar>();
    [SerializeField] GameObject UiInterecty;

    private void OnTriggerEnter(Collider other)
    {
        UiInterecty.SetActive(true);
        conversars.Add(other.gameObject.GetComponent<conversar>());

        if (conversars[conversars.Count-1].ativado)
        {
            conversars.Remove(conversars[conversars.Count - 1]);
            if (conversars.Count <= 0)
            {
                UiInterecty.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        conversars.Remove(other.gameObject.GetComponent<conversar>());
        if (conversars.Count <= 0)
        {
            UiInterecty.SetActive(false);
        }
    }

    public void interectyButtom(InputAction.CallbackContext context)
    {
        if(conversars.Count <= 0)return; 

        if (context.performed)
        {
            conversars[0].dialogo();

            if (conversars[0].ativado)
            {
                conversars.Remove(conversars[0]);
                if (conversars.Count <= 0)
                {
                    UiInterecty.SetActive(false);
                }
            }
        }
        
    }

}
