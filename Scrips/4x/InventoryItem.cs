using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private InventoryManager inventoryManager;
    private Image itemImage;
    private GameObject hoverBox;
    private Equipment equipment;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        inventoryManager = FindObjectOfType<InventoryManager>(); 
        itemImage = GetComponent<Image>();
    }
    void Start() {
        hoverBox = GameObject.FindWithTag("Hoverbox");
        equipment = GetComponentInParent<Equipment>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        transform.SetAsLastSibling(); // Move the item to the front
        itemImage.raycastTarget = false;
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // Update the position of the item based on the drag input
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(hoverBox.GetComponent<Image>().color == new Color (1f, 1f, 1f, 0f)) {
            hoverBox.GetComponent<Image>().color = new Color (1f, 1f, 1f, 1f);
        }
        foreach(Transform child in hoverBox.transform) {
            if(child.name == "Description") {
                child.GetComponent<Text>().text = name;
                hoverBox.transform.position = new Vector2(transform.position.x, eventData.position.y + 75);
            }
            if(child.name == "More") {
               child.GetComponent<Text>().text = $"Attack: {equipment.Attack}\nHealth: {equipment.Health}";
            }
        }
        hoverBox.SetActive(true); // Activate the hover box when mouse enters
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverBox.SetActive(false); // Deactivate the hover box when mouse exits
    }

    public void OnEndDrag(PointerEventData eventData) {
    // Find the nearest cell under the pointer
    GameObject nearestCell = FindNearestCell(eventData.pointerCurrentRaycast.gameObject);
    print("Pointer: " + eventData.pointerCurrentRaycast.gameObject);
    print("Nearest: " + nearestCell.name);
    if (nearestCell != null)
    {
        inventoryManager.AddToCell(nearestCell, transform.gameObject);
    }
    itemImage.raycastTarget = true;
    
}

private GameObject FindNearestCell(GameObject pointerObject)
{
    string[] parts = pointerObject.name.Split(new char[] { '(' }, 2, StringSplitOptions.RemoveEmptyEntries);
    string result = parts[0].Trim();
    
    //Brukes på equiped UI
    if(result != "GridElement") {
        switch(pointerObject.name) {
            case "Equiped Hands" : {
                if(transform.gameObject.GetComponent<Equipment>().Type == "Hands") {
                    inventoryManager.AddToCell(pointerObject, transform.gameObject);
                }
                break;
            }
            case "Equiped Weapon" : {
                if(transform.gameObject.GetComponent<Equipment>().Type == "Weapon") {
                    inventoryManager.AddToCell(pointerObject, transform.gameObject);
                }
                break;
            }
            case "Equiped Head" : {
                if(transform.gameObject.GetComponent<Equipment>().Type == "Head") {
                    inventoryManager.AddToCell(pointerObject, transform.gameObject);
                }
                break;
            }
            case "Equiped Chest" : {
                if(transform.gameObject.GetComponent<Equipment>().Type == "Chest") {
                    inventoryManager.AddToCell(pointerObject, transform.gameObject);
                }
                break;
            }
            case "Equiped Legs" : {
                if(transform.gameObject.GetComponent<Equipment>().Type == "Feet") {
                    inventoryManager.AddToCell(pointerObject, transform.gameObject);
                }
                break;
            }
        }
        inventoryManager.CalculateStats();
        return pointerObject.transform.parent.gameObject; //Husker ikke lenger, men må ha
    }
    inventoryManager.CalculateStats();
    return pointerObject;
}



}
