using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChesthatOpener : MonoBehaviour {

    private Animator myAnimator;
    private Rigidbody rb;
    private DwarfAnimationsScript dwanimscr;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        dwanimscr = gameObject.GetComponentInParent(typeof(DwarfAnimationsScript)) as DwarfAnimationsScript;

    }

    private void Update()
    {
        
        print(dwanimscr.actualAxis);
        myAnimator.SetFloat("Timeline", dwanimscr.actualAxis.z);
    }
}
