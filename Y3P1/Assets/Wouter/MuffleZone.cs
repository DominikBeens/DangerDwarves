using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuffleZone : MonoBehaviour {

    public AudioLowPassFilter parentLowpass;
    private LocalMusicController parentMusic;
    public float desiredLowpassAmount;
    public float lerpTime;
    public bool active;
    

    private void Start()
    {
        parentMusic = parentLowpass.gameObject.GetComponent<LocalMusicController>();
    }

    private void Update()
    {
        if(active)
        {
            parentLowpass.cutoffFrequency = Mathf.Lerp(parentLowpass.cutoffFrequency, desiredLowpassAmount, 1f / lerpTime * Time.deltaTime);
            parentMusic.muffled = true;
        }
        else
        {
            parentLowpass.cutoffFrequency = Mathf.Lerp(parentLowpass.cutoffFrequency, 22000, 1f / lerpTime * Time.deltaTime);
            parentMusic.muffled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.layer == 9)
            {
                if(parentLowpass != null)
                {
                    active = true;
                    
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.layer == 9)
            {
                if (parentLowpass != null)
                {
                    active = false;
                    
                }           
            }
        }
    }
}
