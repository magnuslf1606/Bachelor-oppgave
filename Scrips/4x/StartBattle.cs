using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBattle : MonoBehaviour
{
    public GameObject popupPanel;

    void Start()
    {
        // Skjul popup-vinduet når spillet starter
        popupPanel.SetActive(false);
    }

    // Kalles når spilleren klikker på en spesifikk tile
    public void OnTileClicked()
    {
        // Vis popup-vinduet når en tile klikkes
        popupPanel.SetActive(true);
    }

    // Kalles når spilleren klikker på "Start Battle" -knappen i popup-vinduet
    public void StartBattleButtonClicked()
    {
        SceneManager.LoadScene("Battle"); // Laster inn kampscenen
       
    }
   
    // Kalles når spilleren klikker på "Leave" -knappen i popup-vinduet
    public void LeaveButtonClicked()
    {
        // Skjul popup-vinduet når spilleren forlater
        popupPanel.SetActive(false);
    }
}
