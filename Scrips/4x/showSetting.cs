using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showSetting : MonoBehaviour
{
    public GameObject settingsPanel; // Referanse til panelet som skal vises eller skjules
    public Slider slider1;
    public Slider slider2;

    private void Start()
    {
        settingsPanel.SetActive(false);
    }
    public void ShowSettings()
    {
        // Sjekk om panelet er aktivt eller inaktivt, og motsatt handling
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
    public void ShowHideSettingPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);

        // Skjul eller vis sliderne avhengig av panelets status
        slider1.gameObject.SetActive(false);
        slider2.gameObject.SetActive(settingsPanel.activeSelf);
    }

}
