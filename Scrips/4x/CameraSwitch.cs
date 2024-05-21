using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject FourXCam,FourXObj;
    public GameObject BattlePrefab;
    private GameObject Main;
    private GameObject currentbattle,battleCam;

    void Start()
    {
        Main = FourXCam;
    }
    
    public void swap(){
        if(Main == FourXCam){
            currentbattle = Instantiate(BattlePrefab, transform);
            battleCam = currentbattle.GetComponent<camerainfo>().cam;
            Main = battleCam;
            battleCam.SetActive(true);
            FourXCam.SetActive(false);
            FourXObj.SetActive(false);
            Debug.Log("Swapped to Battle");
        }
        else {
            Main = FourXCam;
            FourXCam.SetActive(true);
            FourXObj.SetActive(true);
            battleCam.SetActive(false);
            Destroy(currentbattle);
            Debug.Log("Swapped to 4x");
        }
    }
}
