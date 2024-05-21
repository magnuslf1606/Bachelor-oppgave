using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpirteAnimator : MonoBehaviour
{
    [SerializeField] private SpirteAnimator[] frameArray;
    int currentFrame;
    float timer;
    // Update is called once per frame
    void Update()
    {
        //timer += Time.delaTime;

        if(timer >= 1f) {
            timer -= 1f;
            currentFrame++;
            //gameObject.GetComponent<SpirteRenderer>().spirte = frameArray[currentFrame];
        }
    }
}
