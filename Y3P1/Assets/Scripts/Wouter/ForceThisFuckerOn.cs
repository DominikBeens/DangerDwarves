using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AuraAPI;

public class ForceThisFuckerOn : MonoBehaviour {

    private AuraLight myAuraLight;

    private void Start()
    {
        myAuraLight = GetComponent<AuraLight>();
    }

    private void Update()
    {
        if(!myAuraLight.isActiveAndEnabled)
        {
            myAuraLight.enabled = true;
        }
    }
}
