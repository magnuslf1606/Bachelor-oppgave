using UnityEngine;

public class BuildDecrease : MonoBehaviour
{
    public int turnsDecreased;
    public int SetBuildTimer(int x) {
        if(x > 0) return x-turnsDecreased;
        return 0;
    }
    
}
