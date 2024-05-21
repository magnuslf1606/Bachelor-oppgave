using UnityEngine;
using UnityEngine.UI;

public class Builder : MonoBehaviour
{
    private BuildDecrease b;
    void Start()
    {
        b = GameObject.FindWithTag("TechScript").GetComponent<BuildDecrease>();
        
    }
    void Update() {
        if(transform.GetChild(4).GetComponent<Text>().text == "") {
            b.enabled = true;
        }
        
    }
    
}
