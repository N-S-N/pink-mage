using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Color = UnityEngine.Color;

public class ControlerMenu : MonoBehaviour
{
    [SerializeField] List<Save> words = new List<Save>();
    [SerializeField] GameObject ButomSave;
    [SerializeField] GameObject PaiSave;
    [SerializeField] RectTransform save;
    [SerializeField] float alementoTamanho = 231.26f;

    #region loud
    private void Awake()
    {
        loud();
       // Debug.Log(save.offsetMin);
    }

    void loud()
    {
        //Puxar o save
        if (File.Exists("menu.json"))
        {
            string jsonData = File.ReadAllText("menu.json");

            SaveData lineMapdafe = JsonUtility.FromJson<SaveData>(jsonData);
            words = lineMapdafe.slotData;
        }
        //server para spanar os save

        for (int i = 0; i < words.Count; i++)
        {
            //spanar
            GameObject Butom = Instantiate(ButomSave, PaiSave.transform);
            SaveControler savecon = Butom.GetComponent<SaveControler>();
            //setar butom
            savecon.tempo = words[i].Tempo;
            savecon.Slot = words[i].sloat;
            savecon.Imagem[0].color = words[i].personagemCapacete;
            savecon.Imagem[1].color = words[i].personagemCapa;
            savecon.Imagem[2].color = words[i].personagemLuva;
            savecon.Imagem[3].color = words[i].personagemBota;

            savecon.fase = words[i].fase;

            //tempo cauculado em horas
            float horas = (words[i].Tempo / 60) / 60;
            float menutos = (words[i].Tempo / 60);
  

            savecon.TempoDeJogo.text = horas.ToString("F0") + ":" + menutos.ToString("F0");
            savecon.SlotDoButom.text = words[i].sloat.ToString();
            savecon.imagemDoPersonagem[0].color = words[i].personagemCapacete;
            savecon.imagemDoPersonagem[1].color = words[i].personagemCapa;
            savecon.imagemDoPersonagem[2].color = words[i].personagemLuva;
            savecon.imagemDoPersonagem[3].color = words[i].personagemBota;

            //almentar o tamanho coretamente

            save.offsetMin -= new Vector2 (0, alementoTamanho);

        }
    }

    public void saveMundo()
    {
        SaveData data = new SaveData();

        for (int i = 0; i < words.Count; i++)
        {
            Save itemdata = new Save(words[i].sloat, words[i].fase, words[i].personagemCapacete, words[i].personagemCapa, words[i].personagemLuva, words[i].personagemBota, words[i].Tempo);
            data.slotData.Add(itemdata);
        }

        string jsonData = JsonUtility.ToJson(data);

        File.WriteAllText("menu.json", jsonData);
    }


    #endregion

    #region butom
    public void Quit()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        words.Add(new Save(words.Count,1,Color.white, Color.white, Color.white, Color.white, 0));
        saveMundo();
        PlayerPrefs.SetInt("voltacombate", 0);
        PlayerPrefs.SetFloat("Tempo", 0);
        PlayerPrefs.SetInt("Carregar", 0);
        PlayerPrefs.SetInt("espašoDeAmazenamento", words.Count-1);
        SceneManager.LoadSceneAsync(1);
    }  

    #endregion
}



[System.Serializable]
public class Save
{
    public int sloat;
    public int fase;
    public Color personagemCapacete;
    public Color personagemCapa;
    public Color personagemLuva;
    public Color personagemBota;
    public float Tempo;

    public Save(int sloat, int fase, Color personagemCapacete, Color personagemCapa, Color personagemLuva, Color personagemBota, float tempo)
    {
        this.sloat = sloat;
        this.fase = fase;
        this.personagemCapacete = personagemCapacete;
        this.personagemCapa = personagemCapa;
        this.personagemLuva = personagemLuva;
        this.personagemBota = personagemBota;

        this.Tempo = tempo;
    }
}

[System.Serializable]
public class SaveData
{
    public List<Save> slotData = new List<Save>();
}

