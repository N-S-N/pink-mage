using TMPro;
using UnityEngine;

public class Iteam : MonoBehaviour
{
    public ItemaConfig ItemConfig;
    public Color imagem;
    public TMP_Text descri��o;
    private void Awake()
    {
        ItemConfig.descricao = descri��o.text;
    }

    public enum TipoIteam
    {
        capacete,
        peitoral,
        calca,
        bolta,
        couro,
        la
    }

}
