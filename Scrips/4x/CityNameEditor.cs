using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityNameEditor : MonoBehaviour
{
    public Text cityNameText;
    public InputField cityNameInputField;
    public Button confirmButton;

    void Start()
    {
        // Legg til en lytter for bekreftelsesknappen
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

 public void OnConfirmButtonClick()
    {
        // Oppdater bynavnet med teksten fra InputField
        UpdateCityName(cityNameInputField.text);
    }

    void UpdateCityName(string newName)
    {
        // Oppdater teksten i bynavnet
        cityNameText.text = "City: " + newName;

        // Tøm InputField etter oppdatering
        cityNameInputField.text = "";
    }
}
