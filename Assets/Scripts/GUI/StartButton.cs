using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartButton : MonoBehaviour
{
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void StartGame()
    {
        gameManager.LoadScene();
    }
}
