using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalMusicController : MonoBehaviour {

    private float desiredVolume;
    private bool iAmCurrentMusic;
    private AudioSource myMusic;
    public float lerpTime;
    [HideInInspector]
    public bool muffled;

    private void Start()
    {
        myMusic = GetComponent<AudioSource>();
        desiredVolume = myMusic.volume;
        myMusic.volume = 0;

        if(desiredVolume == 0)
        {
            desiredVolume = 1;
        }
    }

    private void Update()
    {
        if (iAmCurrentMusic)
        {
            if (muffled)
            {
                myMusic.volume = Mathf.Lerp(myMusic.volume, desiredVolume * 0.5f, 1f / lerpTime * Time.deltaTime);
            }
            else
            {
                myMusic.volume = Mathf.Lerp(myMusic.volume, desiredVolume * 1, 1f / lerpTime * Time.deltaTime);
            }
        }
        else
        {
            myMusic.volume = Mathf.Lerp(myMusic.volume, 0, 1f / lerpTime * Time.deltaTime * 2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.layer == 9)
            {              
                iAmCurrentMusic = true;                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.layer == 9)
            {               
                iAmCurrentMusic = false;               
            }
        }
    }
}
