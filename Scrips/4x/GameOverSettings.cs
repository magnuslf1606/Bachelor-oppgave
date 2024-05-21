using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverSettings : MonoBehaviour
{
    public Button exit, back;
    public Text state;
    // Start is called before the first frame update
    void Start()
    {
        exit.onClick.AddListener(Exit);
        back.onClick.AddListener(Back);
        print("START UP");
        print(PlayerPrefs.GetString("won"));
        if(state) print("Found item");
        if(PlayerPrefs.GetString("won") == "false") {
            state.text = "Defeat";
        }
    }

    // Update is called once per frame
    void Exit() {
        print("Exit");
        Application.Quit();
    }
    void Back() {
        SceneManager.LoadScene(0);
    }
}
