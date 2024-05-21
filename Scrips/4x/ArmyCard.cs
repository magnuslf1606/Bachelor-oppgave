using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyCard : MonoBehaviour
{
    public Text soldiersCountText; // Tekstobjektet som viser antall soldater på kortet
    public Image cardImage; // Bildekomponenten som viser bildet på kortet

    // Metode for å sette antall soldater på kortet
    public void SetSoldiersCount(int count)
    {
        soldiersCountText.text = "Soldiers: " + count.ToString(); // Oppdater teksten på kortet med antall soldater
    }

    // Metode for å sette bilde på kortet
    public void SetCardImage(Sprite image)
    {
        cardImage.sprite = image; // Tilordne det angitte bildet til bildekomponenten på kortet
    }
}
