using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceHover : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hoverBox;
    public void OnPointerEnter(PointerEventData eventData)  {
        
        if (hoverBox.GetComponent<Image>().color == new Color (1f, 1f, 1f, 0f)) {
            hoverBox.GetComponent<Image>().color = new Color (1f, 1f, 1f, 1f);
        }
        foreach (Transform child in hoverBox.transform) {
            if (child.name == "Description") {
                child.GetComponent<Text>().text = name;
                // Adjust position as needed
                hoverBox.transform.position = new Vector2(transform.transform.position.x, hoverBox.transform.position.y);
            }
            if (child.name == "More")  {
                foreach(Transform t in transform) {
                    string[] arr = t.name.Split(' ');
                    if (arr.Length >= 1 && arr[1] == "Text") 
                        child.GetComponent<Text>().text = "Amount : " + t.GetComponent<Text>().text;
                }
            }
        }
        hoverBox.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        hoverBox.SetActive(false);
    }
}
