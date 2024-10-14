using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Iteam;

public class lojaControl : MonoBehaviour
{
    #region variaveis

    [Header("UI and Distany")]
    [SerializeField] float distacy;
    [SerializeField] GameObject Ui;
    [SerializeField] TMP_Text descricaoUi;


    [Header("scripts")]
    [SerializeField] PlayerControler playerControl;
    [SerializeField] Inventario Inventario;

    [Header("obj")]
    [SerializeField] GameObject player;

    [Header("Iteam")]
    public List<IteamBay> iteamLoja = new List<IteamBay>();

    [Header("buttom")]
    public List<EventTrigger> triger;
    public List<rquipamentos> equipamentosQuadado;
    #endregion

    #region funcao

    public void interection(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float disstancy = Vector3.Distance(player.transform.position,transform.position);
            if(disstancy < distacy)
            {
                Ui.SetActive(true);
                updateDaita();
            }
        }
    }

    private void Awake()
    {
        playerControl.UiLoja = Ui;
        //pegando config de iteam
        for (int i = 0; i < iteamLoja.Count; i++)
        {
            Iteam inteam = iteamLoja[i].IteamOBJ.GetComponent<Iteam>();
            inteam.ItemConfig.descricao = inteam.descrição.text;
            //
            iteamLoja[i].Iteam = inteam.ItemConfig;
            iteamLoja[i].Imagem = inteam.imagem;
        }

    }

    void updateDaita()
    {
        //mudar item no inventario
        for (int i = 0 ; i < Inventario.equipamentosQuadado.Count; i++)
        {
            
            equipamentosQuadado[i].ImagemLocal.sprite = Inventario.equipamentosQuadado[i].ImagemLocal.sprite;
            equipamentosQuadado[i].Iteam = Inventario.equipamentosQuadado[i].Iteam;
            equipamentosQuadado[i].texto.text = Inventario.equipamentosQuadado[i].texto.text;
            equipamentosQuadado[i].Imagem = Inventario.equipamentosQuadado[i].Imagem;
            if (Inventario.equipamentosQuadado[i].Imagem.a != 0)
                equipamentosQuadado[i].ImagemLocal.color = Inventario.equipamentosQuadado[i].Imagem;
            else
                equipamentosQuadado[i].ImagemLocal.color = Color.white;
        }

        //inabilidando e habilitando a compra
        for (int i = 0; i < iteamLoja.Count; i++)
        {
            triger[i].enabled = true;
            for (int j = 0; j < iteamLoja[i].bay.Count; j++)
            {
                switch (iteamLoja[i].bay[j].tipo)
                {
                    case TipoIteam.la:
                        if (Inventario.la < iteamLoja[i].bay[j].cost)
                        {
                            triger[i].enabled = false;
                            break;
                        }

                        break;
                    case TipoIteam.couro:
                        if (Inventario.couro < iteamLoja[i].bay[j].cost)
                        {
                            triger[i].enabled = false;
                            break;
                        }

                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void AddRoupaInInventory(int intex)
    {
        Inventario.ColoetarItema(iteamLoja[intex].Iteam, iteamLoja[intex].Imagem);
        updateDaita();
    }

    public void descricaoIteam(int intex)
    {
        descricaoUi.text = iteamLoja[intex].Iteam.descricao;

        //custo
        for (int j = 0; j < iteamLoja[intex].bay.Count; j++)
        {
            switch (iteamLoja[intex].bay[j].tipo)
            {
                case TipoIteam.la:
                    Inventario.la -= iteamLoja[intex].bay[j].cost;

                    break;
                case TipoIteam.couro:
                    Inventario.couro -= iteamLoja[intex].bay[j].cost;

                    break;
                default:
                    break;
            }
        }

    }

    #endregion
}

#region criat a class
[System.Serializable]
public class IteamBay
{
    [Header("Iteam")]
    public GameObject IteamOBJ;
    [HideInInspector] public ItemaConfig Iteam;
    [HideInInspector] public Color Imagem;
    [Header("how much")]
    public List<bayCustom> bay;

    public IteamBay(GameObject IteamOBJ, ItemaConfig Iteam, Color imagem, List<bayCustom> bay)
    {
        this.IteamOBJ = IteamOBJ;
        this.Iteam = Iteam;
        this.Imagem = imagem;
        this.bay = bay;
    }
}

[System.Serializable]
public class bayCustom
{
    public TipoIteam tipo;
    public int cost;
    public bayCustom(TipoIteam tipo, int cost)
    {
        this.tipo = tipo;
        this.cost = cost;
    }
}


#endregion
