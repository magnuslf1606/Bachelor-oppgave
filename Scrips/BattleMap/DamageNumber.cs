using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    private GameObject folder;
    public float destroyDelay = 2f;

    // Start is called before the first frame update
    public void NewText(string x, Vector3 pos)
    {
        
        // Create a new TextMeshPro object
        GameObject damageNumberObject = new GameObject("DamageNumberText");
        TextMeshProUGUI textComponent = damageNumberObject.AddComponent<TextMeshProUGUI>();

        // Set the parent to the folder GameObject (you can adjust the folder object as needed)
        damageNumberObject.transform.SetParent(folder.transform);

        // Set the position
        damageNumberObject.transform.position = pos;

        // Set the text
        textComponent.text = x;

        // Optionally, you can set other properties like font size, color, etc. here

        // Destroy the object after a delay
        Destroy(damageNumberObject, destroyDelay);
        
    }

}
