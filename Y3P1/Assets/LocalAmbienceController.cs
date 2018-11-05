using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class LocalAmbienceController : MonoBehaviour {

    public GameObject playercamera;

    public bool iAmCurrentAmbience;
    public float fogDensity;
    public Color fogColor;
    public float lerpTime;



	// Use this for initialization
	void Start ()
    {
        playercamera = Player.localPlayer.playerCam.gameObject;
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(iAmCurrentAmbience)
        {
            AdjustAmbience();
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(other.gameObject.layer == 9)
            {
                iAmCurrentAmbience = true;
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.layer == 9)
            {
                iAmCurrentAmbience = false;
            }
        }
    }

    public void AdjustAmbience()
    {
        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensity, 1f / lerpTime * Time.deltaTime);
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColor, 1f / lerpTime * Time.deltaTime);
    }
}
