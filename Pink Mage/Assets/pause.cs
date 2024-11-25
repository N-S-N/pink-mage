using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pause : MonoBehaviour
{
    [SerializeField] GameObject pausegame;
    [SerializeField] GameObject settinggame;

    private void OnDisable()
    {
        pausegame.SetActive(true);
        settinggame.SetActive(false);
    }
}
