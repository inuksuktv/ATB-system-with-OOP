using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public List<string> heroNames = new List<string>();
    public TMP_InputField input1;
    public TMP_InputField input2;
    public TMP_InputField input3;
    public TMP_InputField input4;

    private void Awake()
    {
        // Destroy myself if there's an instance that isn't me, otherwise persist through scenes.
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadScene()
    {
        heroNames.Add(input1.text);
        heroNames.Add(input2.text);
        heroNames.Add(input3.text);
        heroNames.Add(input4.text);
        SceneManager.LoadScene("BattleScene");
    }
}
