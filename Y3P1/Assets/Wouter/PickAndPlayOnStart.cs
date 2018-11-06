using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAndPlayOnStart : MonoBehaviour {

    public List<AudioSource> SFXList = new List<AudioSource>();

    private void Start()
    {
        SFXList[Random.Range(0, SFXList.Count)].Play();
    }
}
