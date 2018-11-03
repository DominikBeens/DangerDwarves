using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedSoundPlayer : MonoBehaviour {
    public float lifetime;
    public bool randomizePitch;
    public float minPitch;
    public float maxPitch;
    public AudioSource myAudioSource;

	// Use this for initialization
	void Start ()
    {
        if (lifetime == 0)
        {
            lifetime = 2;
        }
        if (minPitch == 0 )
        {
            minPitch = 0.9f;
        }
        if(maxPitch == 0)
        {
            maxPitch = 1.1f;
        }
        transform.parent = null;
        
        if(randomizePitch)
        {
            myAudioSource.pitch = Random.Range(minPitch, maxPitch);
        }

        Destroy(this.gameObject, lifetime);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
