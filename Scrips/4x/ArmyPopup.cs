using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyPopup : MonoBehaviour
{
    public GameObject army;
 
    // Start is called before the first frame update
    void Start()
    {
        army.SetActive(false);

    }

  
    void OnMouseDown()
    {
        // Destroy the gameObject after clicking on it
        army.SetActive(!army.activeSelf);
    }
   
}
