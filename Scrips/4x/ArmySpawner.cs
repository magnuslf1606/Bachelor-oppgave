using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmySpawner : MonoBehaviour
{
    public List<GameObject> armyCardsPrefabs; // Liste over prefabs for de ulike soldatene
    public Transform parentObject;
    // Objektet hvor armyCards skal spawnes
    public float xOffset;// Offset for å plassere det nye objektet ved siden av det eksisterende
    public Dropdown soldierDropdown;
    private Vector3 newPosition;
    //Metode for å opprette alle army cards
    public void SpawnArmyCards()
    {
        if (armyCardsPrefabs != null && armyCardsPrefabs.Count > 0 && parentObject != null)
        {
            // Hent indeksen til den valgte soldat- eller fiendetype fra dropdown-menyen
            int selectedSoldierIndex = soldierDropdown.value;

            // Sjekk at indeksen er innenfor gyldige grenser
            if (selectedSoldierIndex >= 0 && selectedSoldierIndex < armyCardsPrefabs.Count)
            {
                // Bruk indeksen til å velge riktig prefab fra listen
                GameObject selectedPrefab = armyCardsPrefabs[selectedSoldierIndex];

                // Beregn posisjonen for det neste kortet
                Vector3 newPosition = CalculateNextCardPosition();

                // Opprett det nye armyCards-objektet
                GameObject newArmyCards = Instantiate(selectedPrefab, newPosition, Quaternion.identity, parentObject);

                // Sett riktig transformasjon for det nye armyCards-objektet
                newArmyCards.transform.localPosition = newPosition;
                newArmyCards.transform.localRotation = Quaternion.identity;
                newArmyCards.transform.localScale = Vector3.one;
            }
            else
            {
                Debug.LogWarning("Invalid selected soldier index.");
            }
        }
        else
        {
            Debug.LogWarning("Missing army cards prefabs list or parent object, or the list is empty.");
        }
    }

    private Vector3 CalculateNextCardPosition()
    {
        // Hent antall barn i parentObject
        int childCount = parentObject.childCount;

        if (childCount == 0)
        {
            // Hvis det ikke er noen kort ennå, plasser det første kortet til venstre for parentObject
            newPosition = parentObject.position;
        }
        else
        {
            // Hent posisjonen til det siste kortet
            Transform lastChild = parentObject.GetChild(childCount - 1);

            // Beregn posisjonen for det neste kortet basert på den forrige kortet og multiplikatoren for xOffset
            newPosition = lastChild.position + lastChild.right * xOffset * childCount;

        }
        return newPosition;
    }

    // Metode for å fjerne alle eksisterende army cards
    public void ClearArmyCards()
    {
        // Fjern alle eksisterende army cards fra parentObject
        foreach (Transform child in parentObject)
        {
            Destroy(child.gameObject);
        }
    }
}
