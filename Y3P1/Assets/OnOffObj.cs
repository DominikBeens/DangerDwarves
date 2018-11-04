using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffObj : MonoBehaviour {

    public GameObject obj;

    public void TurnOnObj()
    {
        obj.SetActive(true);
    }

    public void TurnOffObj()
    {
        obj.SetActive(false);
    }
}

