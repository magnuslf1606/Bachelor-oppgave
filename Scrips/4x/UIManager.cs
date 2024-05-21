using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Canvas gameCanvas, battleCanvas, techTree, settings;
    public Transform hexes;
    private bool building = false;
    public Controls controls;

    private void Start()
    {
        //canvas.gameObject.SetActive(false);
    }
    void Update() {

        //Når i meny pauser spillet og hit detection på bakgrund er skrudd av
        if(gameCanvas.isActiveAndEnabled || battleCanvas.isActiveAndEnabled || building || techTree.isActiveAndEnabled || settings.isActiveAndEnabled) {
            DisableColliders(false);
        } else {
            DisableColliders(true);
        }
    }

    void DisableColliders(bool x) {
        controls.enabled = x;
    }
}
