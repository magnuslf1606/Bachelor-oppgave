using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    
    public GameObject hoverBox;
    public CityManagement cityManagement;
 
    public void OnPointerEnter(PointerEventData eventData)
{
    if (name != "Item 0: Buildings") 
    {
        if (hoverBox.GetComponent<Image>().color == new Color (1f, 1f, 1f, 0f)) 
        {
            hoverBox.GetComponent<Image>().color = new Color (1f, 1f, 1f, 1f);
        }
        foreach (Transform child in hoverBox.transform) 
        {
            if (child.name == "Description") 
            {
                child.GetComponent<Text>().text = name.Substring(8);
                // Adjust position as needed
                hoverBox.transform.position = new Vector2(hoverBox.transform.position.x, transform.transform.position.y);
            }
            if (child.name == "More") 
            {
                string costText = "Cost:\n";

                // Get the index of the option in the dropdown based on the name of the GameObject
                int index = -1;
                for (int i = 0; i < cityManagement.buildingDD.options.Count-1; i++)
                {
                    if (cityManagement.buildingDD.options[i+1].text == name.Substring(8))
                    {
                        index = i;
                        break;
                    }
                }

                // If the index is valid, get the corresponding building cost
                if (index != -1 && index < cityManagement.buildingCost.Count) 
                {
                    costText += "Gold: " + cityManagement.buildingCost[index];
                }

                child.GetComponent<Text>().text = costText;
            }
        }
        hoverBox.SetActive(true);
    }
}

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverBox.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        hoverBox.SetActive(false);
    }
}
