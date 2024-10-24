using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class controlerSpaner : MonoBehaviour
{
    public List<spawm> spawms;

    [Header("spawm")]

    [SerializeField] public List<GameObject> prefebEnimy;

    [HideInInspector] public List<combaterSenaslot> combaterprerfebenimy;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("voltacombate") == 1)
        {
            Load();
        }

    }

    void Load()
    {
        
        if (File.Exists("combater.json"))
        {
            string jsonData = File.ReadAllText("combater.json");

            combaterSena lineMapdafe = JsonUtility.FromJson<combaterSena>(jsonData);

            combaterprerfebenimy = lineMapdafe.enimy.enimy;
           
            for (int i = 0;i < combaterprerfebenimy.Count; i++)
            {
                if (combaterprerfebenimy[i].lifeincombater == -1)
                {
                    spawms[combaterprerfebenimy[i].spwnar].spawmactivete(prefebEnimy[combaterprerfebenimy[i].enimy], combaterprerfebenimy[i].positiom);
                }
                
                else if (combaterprerfebenimy[i].lifeincombater == 0)
                {
                    spawms[combaterprerfebenimy[i].spwnar].spawmactivete(null, Vector3.zero);
                }
                
            }
        }

        for(int i = 0; i < spawms.Count; i++)
        {
            spawms[i].issapner = true;
        }
    }
}
