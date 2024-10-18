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
    [SerializeField] RectTransform trasformUi;
    [SerializeField] float alementoTamanho;
    [SerializeField] EventSystem system;
    [SerializeField] GameObject slider;
    [SerializeField] TMP_Text[] material;
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
    public List<butomLojaControl> ButtomControler;


    private bool bayIteam = false;
    Scrollbar scrol;
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
        trasformUi.offsetMin = new Vector2(trasformUi.offsetMin.x,0);
        for (int i = 0; i < iteamLoja.Count; i++)
        {
            Iteam inteam = iteamLoja[i].IteamOBJ.GetComponent<Iteam>();
            inteam.ItemConfig.descricao = inteam.descrição.text;
            //
            iteamLoja[i].Iteam = inteam.ItemConfig;
            iteamLoja[i].Imagem = inteam.imagem;

            //conficurar os item da loja
            ButtomControler[i].controler.Nome.text = inteam.ItemConfig.Nome;
            ButtomControler[i].controler.Imagem.color = inteam.imagem;
            ButtomControler[i].controler.Imagem.sprite = Inventario.conficOfColor(inteam.ItemConfig.tipo);

            for (int j = 0; j < ButtomControler[i].controler.bay.Count; j++)
            {
                if (iteamLoja[i].bay.Count-1 !< j)
                {
                    ButtomControler[i].controler.bay[j].imagem.enabled = false;
                }
                else
                {
                    ButtomControler[i].controler.bay[j].imagem.sprite = Inventario.conficOfColor(iteamLoja[i].bay[j].tipo);
                    ButtomControler[i].controler.bay[j].quantidade.text = iteamLoja[i].bay[j].cost.ToString();
                }
            }
            trasformUi.offsetMin -= new Vector2(0, alementoTamanho);
        }
        scrol = slider.GetComponent<Scrollbar>();
    }

    void updateDaita()
    {
        //mudar item no inventario
        for (int i = 0; i < Inventario.equipamentosQuadado.Count; i++)
        {

            equipamentosQuadado[i].ImagemLocal.sprite = Inventario.equipamentosQuadado[i].ImagemLocal.sprite;
            equipamentosQuadado[i].Iteam = Inventario.equipamentosQuadado[i].Iteam;
            equipamentosQuadado[i].texto.text = Inventario.equipamentosQuadado[i].texto.text;
            equipamentosQuadado[i].Imagem = Inventario.equipamentosQuadado[i].Imagem;
            equipamentosQuadado[i].ImagemLocal.preserveAspect = true;
            Inventario.equipamentosQuadado[i].ImagemLocal.preserveAspect = true;
            if (Inventario.equipamentosQuadado[i].Imagem.a != 0)
                equipamentosQuadado[i].ImagemLocal.color = Inventario.equipamentosQuadado[i].Imagem;
            else
                equipamentosQuadado[i].ImagemLocal.color = Color.white;
        }

        //inabilidando e habilitando a compra
        for (int i = 0; i < iteamLoja.Count; i++)
        {
            triger[i].enabled = true;
            ButtomControler[i].controler.FundoDeIndisponibilidade.gameObject.SetActive(false);
            for (int j = 0; j < iteamLoja[i].bay.Count; j++)
            {
                switch (iteamLoja[i].bay[j].tipo)
                {
                    case TipoIteam.la:
                        if (Inventario.la < iteamLoja[i].bay[j].cost)
                        {
                            triger[i].enabled = false;
                            ButtomControler[i].controler.FundoDeIndisponibilidade.gameObject.SetActive(true);
                            ButtomControler[i].controler.bay[j].imagem.color = Color.red;
                            break;
                        }
                        else
                        {
                            ButtomControler[i].controler.bay[j].imagem.color = Color.white;
                        }
                        break;
                    case TipoIteam.couro:
                        if (Inventario.couro < iteamLoja[i].bay[j].cost)
                        {
                            triger[i].enabled = false;
                            ButtomControler[i].controler.FundoDeIndisponibilidade.gameObject.SetActive(true);
                            ButtomControler[i].controler.bay[j].imagem.color = Color.red;
                            break;
                        }
                        else
                        {
                            ButtomControler[i].controler.bay[j].imagem.color = Color.white;
                        }
                        break;
                    case TipoIteam.coracao_Envenenado:
                        if (Inventario.coracao_Envenenado < iteamLoja[i].bay[j].cost)
                        {
                            triger[i].enabled = false;
                            ButtomControler[i].controler.FundoDeIndisponibilidade.gameObject.SetActive(true);
                            ButtomControler[i].controler.bay[j].imagem.color = Color.red;
                            break;
                        }
                        else
                        {
                            ButtomControler[i].controler.bay[j].imagem.color = Color.white;
                        }
                        break;
                    case TipoIteam.cranio:
                        if (Inventario.cranio < iteamLoja[i].bay[j].cost)
                        {
                            triger[i].enabled = false;
                            ButtomControler[i].controler.FundoDeIndisponibilidade.gameObject.SetActive(true);
                            ButtomControler[i].controler.bay[j].imagem.color = Color.red;
                            break;
                        }
                        else
                        {
                            ButtomControler[i].controler.bay[j].imagem.color = Color.white;
                        }
                        break;
                    case TipoIteam.lingua:
                        if (Inventario.lingua < iteamLoja[i].bay[j].cost)
                        {
                            triger[i].enabled = false;
                            ButtomControler[i].controler.FundoDeIndisponibilidade.gameObject.SetActive(true);
                            ButtomControler[i].controler.bay[j].imagem.color = Color.red;
                            break;
                        }
                        else
                        {
                            ButtomControler[i].controler.bay[j].imagem.color = Color.white;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        //numero do material

        material[0].text = Inventario.la.ToString();
        material[1].text = Inventario.couro.ToString();
        material[2].text = Inventario.coracao_Envenenado.ToString();
        material[3].text = Inventario.cranio.ToString();
        material[4].text = Inventario.lingua.ToString();

    }

    private void Update()
    {
        if (!bayIteam) return;
        if (system.currentSelectedGameObject != slider)
        {
            scrol.image.color = Color.white;
            bayIteam = false;
        }

    }

    public void AddRoupaInInventory(int intex)
    {
        Inventario.ColoetarItema(iteamLoja[intex].Iteam, iteamLoja[intex].Imagem);

        system.SetSelectedGameObject(slider);
        
        scrol.image.color = scrol.colors.selectedColor;

        bayIteam = true;

        ButtomControler[intex].gameObject.SetActive(false);
        trasformUi.offsetMin += new Vector2(0, alementoTamanho);

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
                case TipoIteam.coracao_Envenenado:
                    Inventario.coracao_Envenenado -= iteamLoja[intex].bay[j].cost;

                    break;
                case TipoIteam.cranio:
                    Inventario.cranio -= iteamLoja[intex].bay[j].cost;

                    break;
                case TipoIteam.lingua:
                    Inventario.lingua -= iteamLoja[intex].bay[j].cost;

                    break;
                default:
                    break;
            }
        }

        updateDaita();
        
    }

    public void descricaoIteam(int intex)
    {
        descricaoUi.text = iteamLoja[intex].Iteam.descricao;
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
