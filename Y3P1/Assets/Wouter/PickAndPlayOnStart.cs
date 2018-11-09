using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAndPlayOnStart : MonoBehaviour {

    public List<AudioSource> SFXList = new List<AudioSource>();
    public bool useFunctionInstead;

    private void Start()
    {
        if(!useFunctionInstead)
        {
            SFXList[Random.Range(0, SFXList.Count)].Play();
        }
        
    }

    public void Function()
    {

        SFXList[Random.Range(0, SFXList.Count)].Play();
    }
}
