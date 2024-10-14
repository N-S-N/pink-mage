
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static characterBasics;
using static Iteam;

public class Inventario : MonoBehaviour
{
    [Header("spaço de equipamentos")]
    public List<rquipamentos> equipamentosUnsando = new List<rquipamentos>();
    public List<rquipamentos> equipamentosQuadado = new List<rquipamentos>();

    private rquipamentos subistuir, adiquirir;

    [Header("UI de Interectiom")]
    public List<GameObject> Iterface = new List<GameObject>();
    public List<GameObject> IterfaceEquipavel = new List<GameObject>();
    [SerializeField]public List<Image> referencias = new List<Image>(); 
    public TMP_Text[] NomeAtteck;

    [Header("Texto dos equipamentos")]
    [SerializeField]TMP_Text laText, courotext,descricao;
    [SerializeField] GameObject descricaoObj;

    [Header("Materia prima")]
    public int la;
    public int couro;

    [Header("ropa Inicial")]
    [SerializeField] GameObject capacete;
    [SerializeField] GameObject peitora, calca, bota;

    [Header("imagem Ropa")]
    [SerializeField]public Sprite capeceteStripe, peitoralStripe, calcaStripe, boltaStripe;

    [Header("Renderer")]
    [SerializeField]public List<SpriteRenderer> roupaPersonagem;

    private PlayerControler player;

    private void Start()
    {
        player = GetComponent<PlayerControler>();
        if (PlayerPrefs.GetInt("Carregar") == 0) 
        {
            addIteamInicial();
            Destroy(capacete);
            Destroy(peitora);
            Destroy(calca);
            Destroy(bota);
        }
        else
        {
            Destroy(capacete);
            Destroy(peitora);
            Destroy(calca);
            Destroy(bota);
            laText.text = la.ToString();
            courotext.text = couro.ToString();
            return;
        }  

        for (int i = 0; i < equipamentosQuadado.Count; i++)
        {
            if (equipamentosQuadado[i].Imagem == null)
            {
                equipamentosQuadado[i].texto.text = "Sem Iteam";
            }
            else
            {
                equipamentosQuadado[i].texto.text = equipamentosQuadado[i].Iteam.Nome;
            }
        }
        laText.text = la.ToString();
        courotext.text = couro.ToString();
    }
    
    void addIteamInicial()
    {

        equipamentosUnsando[0].Iteam = capacete.GetComponent<Iteam>().ItemConfig;
        equipamentosUnsando[0].Imagem = capacete.GetComponent<Iteam>().imagem;
        equipamentosUnsando[0].texto.text = equipamentosUnsando[0].Iteam.Nome;

        equipamentosUnsando[1].Iteam = peitora.GetComponent<Iteam>().ItemConfig;
        equipamentosUnsando[1].Imagem = peitora.GetComponent<Iteam>().imagem;
        equipamentosUnsando[1].texto.text = equipamentosUnsando[1].Iteam.Nome;

        equipamentosUnsando[2].Iteam = calca.GetComponent<Iteam>().ItemConfig;
        equipamentosUnsando[2].Imagem = calca.GetComponent<Iteam>().imagem;
        equipamentosUnsando[2].texto.text = equipamentosUnsando[2].Iteam.Nome;

        equipamentosUnsando[3].Iteam = bota.GetComponent<Iteam>().ItemConfig;
        equipamentosUnsando[3].Imagem = bota.GetComponent<Iteam>().imagem;
        equipamentosUnsando[3].texto.text = equipamentosUnsando[3].Iteam.Nome;

        for (int i = 0; i < equipamentosUnsando.Count; i ++) 
        {
            addItemaPlayer(equipamentosUnsando[i].Iteam, equipamentosUnsando[i].Imagem, i);
            
        }
    }

    public void ColoetarItema(ItemaConfig Iteam, Color Imagem)
    {
        switch (Iteam.tipo)
        {
            case TipoIteam.la:
                la++;
                laText.text = la.ToString();
                break;
            case TipoIteam.couro:
                couro++;
                courotext.text = couro.ToString();
                break;
            default:
                for (int i = 0; i < equipamentosQuadado.Count; i++)
                {
                    if (equipamentosQuadado[i].Imagem == null)
                    {
                        equipamentosQuadado[i].Iteam = Iteam;
                        equipamentosQuadado[i].Imagem = Imagem;
                        equipamentosQuadado[i].texto.text = equipamentosQuadado[i].Iteam.Nome;

                        switch (equipamentosQuadado[i].Iteam.tipo)
                        {
                            case TipoIteam.capacete:
                                equipamentosQuadado[i].ImagemLocal.sprite = capeceteStripe;

                                break;
                            case TipoIteam.peitoral:
                                equipamentosQuadado[i].ImagemLocal.sprite = peitoralStripe;

                                break;
                            case TipoIteam.calca:
                                equipamentosQuadado[i].ImagemLocal.sprite = calcaStripe;

                                break;
                            case TipoIteam.bolta:
                                equipamentosQuadado[i].ImagemLocal.sprite = boltaStripe;

                                break;
                            default:
                                break;
                        }
                        break;
                    }
                }

                break;
        }
    }

    void addItemaPlayer(ItemaConfig Iteam, Color Imagem, int lugar = 0)
    {
        #region colocando no player

        //colocando  os atributos
        player.Life += Iteam.Life;
        player.Mana += Iteam.Mana;
        player.Medo += Iteam.Medo;

        //adicionando o attack
        player.atteck.Add(Iteam.atteck);

        //adicionando bunusDamege
        for (int i = 0; i < Iteam.bunusDamege.Count; i++)
        {
            player.bunusDamege.Add(Iteam.bunusDamege[i]);
        }
        //adicionando bunusResistem
        for (int i = 0; i < Iteam.bunusResistem.Count; i++)
        {
            player.bunusResistem.Add(Iteam.bunusResistem[i]);
        }
        //adicionando Immunidades
        for (int i = 0; i < Iteam.Immunidades.Count; i++)
        {
            player.Immunidades.Add(Iteam.Immunidades[i]);
        }
        //adicionando Vulnerabilities
        for (int i = 0; i < Iteam.Vulnerabilities.Count; i++)
        {
            player.Vulnerabilities.Add(Iteam.Vulnerabilities[i]);
        }
        //adicionando Resistances
        for (int i = 0; i < Iteam.Resistances.Count; i++)
        {
            player.Resistances.Add(Iteam.Resistances[i]);
        }
        //muddar o nome
        NomeAtteck[lugar].text = Iteam.atteck.Nome;

        //mudando a cor

        for (int i = 0; i < roupaPersonagem.Count; i++)
        {
            roupaPersonagem[i].color = equipamentosUnsando[i].Imagem;
            equipamentosUnsando[i].ImagemLocal.color = equipamentosUnsando[i].Imagem;
            referencias[i].color = equipamentosUnsando[i].Imagem;
        }
        #endregion
    }

    public void SubisituirEquipamento(int positiom)
    {
        if (equipamentosQuadado[positiom].Imagem != null)
        {
            adiquirir = new rquipamentos(equipamentosQuadado[positiom].Iteam,
                                         equipamentosQuadado[positiom].Imagem,
                                         equipamentosQuadado[positiom].texto);
            switch (equipamentosQuadado[positiom].Iteam.tipo)
            {
                case TipoIteam.capacete:
                    subistuir = new rquipamentos(equipamentosUnsando[0].Iteam,
                                                 equipamentosUnsando[0].Imagem,
                                                 equipamentosUnsando[0].texto);
                    TIraEquipamento(equipamentosUnsando[0].Iteam, positiom, 0);
                    equipamentosQuadado[positiom].ImagemLocal.sprite = capeceteStripe;

                    break;
                case TipoIteam.peitoral:
                    subistuir = new rquipamentos(equipamentosUnsando[1].Iteam,
                                                 equipamentosUnsando[1].Imagem,
                                                 equipamentosUnsando[1].texto);
                    TIraEquipamento(equipamentosUnsando[1].Iteam, positiom, 1);
                    equipamentosQuadado[positiom].ImagemLocal.sprite = peitoralStripe;

                    break;
                case TipoIteam.calca:
                    subistuir = new rquipamentos(equipamentosUnsando[2].Iteam,
                                                 equipamentosUnsando[2].Imagem,
                                                 equipamentosUnsando[2].texto);
                    TIraEquipamento(equipamentosUnsando[2].Iteam, positiom, 2);
                    equipamentosQuadado[positiom].ImagemLocal.sprite = calcaStripe;

                    break;
                case TipoIteam.bolta:
                    subistuir = new rquipamentos(equipamentosUnsando[3].Iteam,
                                                 equipamentosUnsando[3].Imagem,
                                                 equipamentosUnsando[3].texto);
                    TIraEquipamento(equipamentosUnsando[3].Iteam, positiom, 3);
                    equipamentosQuadado[positiom].ImagemLocal.sprite = boltaStripe;

                    break;
                default:
                    break;
            }
        }
    }

    public void descricaoFunciom(int positiom)
    {
        if (positiom >= 0)
        {
            if (equipamentosQuadado[positiom].Imagem != null) 
            {
                descricao.text += equipamentosQuadado[positiom].Iteam.descricao;

                descricaoObj.SetActive(true);
            }
        }
        else
        {
            positiom *= -1;
            positiom -= 1;

            descricao.text = equipamentosUnsando[positiom].Iteam.descricao;

            descricaoObj.SetActive(true);
        }
    }

    public void TIraEquipamento(ItemaConfig Iteam, int positionEquipamento, int positiomSubsituir)
    {
        #region tira do player
        //timunuir os atributos
        player.Life -= Iteam.Life;
        player.Mana -= Iteam.Mana;
        player.Medo -= Iteam.Medo;

        //remover o attack
        player.atteck.Remove(Iteam.atteck);

        //remover bunusDamege
        for (int i = 0; i < Iteam.bunusDamege.Count; i++)
        {
            player.bunusDamege.Remove(Iteam.bunusDamege[i]);
        }
        //remover bunusResistem
        for (int i = 0; i < Iteam.bunusResistem.Count; i++)
        {
            player.bunusResistem.Remove(Iteam.bunusResistem[i]);
        }
        //remover Immunidades
        for (int i = 0; i < Iteam.Immunidades.Count; i++)
        {
            player.Immunidades.Remove(Iteam.Immunidades[i]);
        }
        //remover Vulnerabilities
        for (int i = 0; i < Iteam.Vulnerabilities.Count; i++)
        {
            player.Vulnerabilities.Remove(Iteam.Vulnerabilities[i]);
        }
        //remover Resistances
        for (int i = 0; i < Iteam.Resistances.Count; i++)
        {
            player.Resistances.Remove(Iteam.Resistances[i]);
        }
        #endregion

        #region mudando valores

        equipamentosUnsando[positiomSubsituir] = new rquipamentos(adiquirir.Iteam, adiquirir.Imagem, subistuir.texto, equipamentosUnsando[positiomSubsituir].ImagemLocal);
        equipamentosQuadado[positionEquipamento] = new rquipamentos(subistuir.Iteam, subistuir.Imagem, adiquirir.texto, equipamentosQuadado[positionEquipamento].ImagemLocal);
        equipamentosQuadado[positionEquipamento].texto.text = equipamentosQuadado[positionEquipamento].Iteam.Nome;
        equipamentosUnsando[positionEquipamento].texto.text = equipamentosUnsando[positionEquipamento].Iteam.Nome;
        equipamentosQuadado[positionEquipamento].ImagemLocal.color = equipamentosQuadado[positionEquipamento].Imagem;
        equipamentosUnsando[positionEquipamento].ImagemLocal.color = equipamentosUnsando[positionEquipamento].Imagem;

        #endregion

        addItemaPlayer(adiquirir.Iteam, adiquirir.Imagem, positiomSubsituir);
    }

    public void AtiveteInterfece(int posintiom)
    {
        if (posintiom >= 0)
        {
            if (equipamentosQuadado[posintiom].Imagem != null)
            {
                Iterface[posintiom].SetActive(true);
            }
        }
        else
        {
            posintiom *= -1;
            posintiom -= 1;
            IterfaceEquipavel[posintiom].SetActive(true);
        }
    }
}

[System.Serializable]
public class ItemaConfig 
{
    [Header("Nome")]
    public string Nome;

    [Header("variaves padrão")]
    public float Life;
    public float Mana;
    public float Medo;

    [Header("tipo do iteam")]
    public TipoIteam tipo;

    [Header("descrecãos")]
    [HideInInspector]public string descricao;

    [Header("ataque")]
    public atteck atteck;

    [Header("Bonus Elementar")]
    public List<BonusDamed> bunusDamege = new List<BonusDamed>();
    public List<BonusDamed> bunusResistem = new List<BonusDamed>();

    [Header("Restistencias A elemnetos")]
    public List<element> Immunidades = new List<element>();
    public List<element> Vulnerabilities = new List<element>();
    public List<element> Resistances = new List<element>();
    public ItemaConfig(float Life,float Mana,float Medo, TipoIteam Tipo, string descricao,atteck attack, List<BonusDamed> bunusDamege, List<BonusDamed> bunusResistem, List<element> Immunidades, List<element> Vulnerabilities, List<element> Resistances , string nome)
    {
        this.Nome = nome;
        this.Life = Life;
        this.Mana = Mana;
        this.Medo = Medo;
        this.tipo = Tipo;
        this.descricao = descricao;
        this.atteck = attack;
        this.bunusDamege = bunusDamege;
        this.bunusResistem = bunusResistem;
        this.Immunidades = Immunidades;
        this.Vulnerabilities = Vulnerabilities;
        this.Resistances = Resistances;
    }

    public bool StartsWith(ItemaConfig e)
    {
        if (e.Nome == Nome
            && e.Life == Life
            && e.Mana == Mana
            && e.Medo == Medo
            && e.tipo == tipo
            && e.descricao == descricao
            && e.atteck == atteck
            && e.bunusDamege == bunusDamege
            && e.bunusResistem == bunusResistem
            && e.Immunidades == Immunidades
            && e.Vulnerabilities == Vulnerabilities
            && e.Resistances == Resistances)
            return true;
        else
            return false;
    }
}

[System.Serializable]
public class rquipamentos
{
    public ItemaConfig Iteam;
    public Color Imagem;
    public TMP_Text texto;
    public Image ImagemLocal;

    public rquipamentos(ItemaConfig Iteam, Color imagem, TMP_Text textoIteam,Image ImagemLocal = null)
    {
        this.Iteam = Iteam;
        this.Imagem = imagem;
        this.texto = textoIteam;
        this.ImagemLocal = ImagemLocal;
    }
}
