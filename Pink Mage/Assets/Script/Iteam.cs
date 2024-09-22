using TMPro;
using UnityEngine;

public class Iteam : MonoBehaviour
{
    public ItemaConfig ItemConfig;
    public Color imagem;
    public TMP_Text descrição;
    private void Awake()
    {
        ItemConfig.descricao = descrição.text;
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
