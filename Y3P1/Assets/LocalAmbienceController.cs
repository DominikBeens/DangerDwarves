using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class LocalAmbienceController : MonoBehaviour {

    public GameObject playercamera;

    public VolumetricFog myFogSettings;

    public float fogNoiseAmount;
    public float fogFoggyness;

	// Use this for initialization
	void Start ()
    {
        playercamera = Player.localPlayer.playerCam.gameObject;
        myFogSettings = playercamera.GetComponent<VolumetricFog>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
