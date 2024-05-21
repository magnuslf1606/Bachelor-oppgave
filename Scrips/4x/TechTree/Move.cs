using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
   
    void Update() {
        if(transform.GetChild(4).GetComponent<Text>().text == "") {
            //Her kommer øker man movemnt i annet script
        }
        
    }
    
}
