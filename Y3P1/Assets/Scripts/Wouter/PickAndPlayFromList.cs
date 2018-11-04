using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAndPlayFromList : MonoBehaviour {

    //public string listName;

    public List<AudioSource> SFXSwingSounds = new List<AudioSource>();


    public void PlaySFXSwingSoundFromList()
    {
        SFXSwingSounds[Random.Range(0, SFXSwingSounds.Count)].Play();
    }
}
