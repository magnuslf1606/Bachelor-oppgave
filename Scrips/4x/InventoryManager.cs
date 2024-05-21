using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Transform grid; // Reference to the grid's transform
    public Transform EquipedSlots;
    private float ExtraAttack, ExtraHealth;
    public Text attackText, healthText;
    public GameObject charBattle;    
    public Button close, open;
    void Start() {
        open.onClick.AddListener(OpenCanvas);
        close.onClick.AddListener(CloseCanvas);
        
    }
    public void AddItemImages(GameObject prefab) {
        foreach (Transform g in grid) {
            if (g.childCount == 0) {
                var i = Instantiate(prefab, g);
                print(i.transform.name);
                i.transform.name = i.transform.name.Substring(0, i.transform.name.Length-7);
                print(i.transform.name);
                return;
            }
        }
    }
    void OpenCanvas() {
        GetComponent<Canvas>().enabled = true;
    }
    void CloseCanvas() {
        GetComponent<Canvas>().enabled = false;
    }
    public void AddToCell(GameObject cell, GameObject item) {
        string[] parts = item.transform.parent.name.Split(new char[] { '(' }, 2, StringSplitOptions.RemoveEmptyEntries);
        string result = parts[0].Trim();
        string[] parts1 = cell.name.Split(new char[] { '(' }, 2, StringSplitOptions.RemoveEmptyEntries);
        string result1 = parts1[0].Trim();
        if(cell.transform.childCount == 0) {
            item.transform.SetParent(cell.transform);
            item.transform.position = cell.transform.position;
        }
        
        else if (cell.transform.childCount > 0) {
            //Swaping only for the inventorty UI
            if(result == "GridElement" && result1 == "GridElement")
                Swap(cell, item.transform.parent.gameObject);
            else 
                item.transform.position = item.transform.parent.position;
        }
        
        attackText.text = "Extra Attack : " + ExtraAttack;
        healthText.text = ExtraHealth + " : Extra Health ";
    }   
    public void Swap(GameObject start, GameObject newLoc) {
        Debug.Log("Swap Start");

        // Get the parent transforms of the children to be swapped
        Transform startParent = start.transform.parent;
        Transform newLocParent = newLoc.transform.parent;

        // Get the indices of the children in their respective parents
        int startIndex = start.transform.GetSiblingIndex();
        int newLocIndex = newLoc.transform.GetSiblingIndex();

        Debug.Log("Item: " + start.name);
        Debug.Log("newLoc: " + newLoc.name);

        // Swap the children by changing their parent and sibling indices
        start.transform.SetParent(newLocParent);
        start.transform.SetSiblingIndex(newLocIndex);

        newLoc.transform.SetParent(startParent);
        newLoc.transform.SetSiblingIndex(startIndex);

        //Resetter pos på barn
        foreach(Transform child in start.transform) {
            child.transform.position = start.transform.position;
        }
        foreach(Transform child in newLoc.transform) {
            child.transform.position = newLoc.transform.position;
        }
    }


    public void CalculateStats() {
        ExtraAttack = 0;
        ExtraHealth = 0;
        foreach (Transform slot in EquipedSlots) {
            foreach (Transform g in slot) {
                if(g.gameObject.GetComponent<Equipment>().Attack != 0) 
                    ExtraAttack += g.gameObject.GetComponent<Equipment>().Attack;
                
                if(g.gameObject.GetComponent<Equipment>().Health != 0) 
                    ExtraHealth += g.gameObject.GetComponent<Equipment>().Health;
            }
        }
    }
    public float GetAttack() { CalculateStats(); return ExtraAttack; }
    public float GetHealth() { CalculateStats(); return ExtraHealth; }
}