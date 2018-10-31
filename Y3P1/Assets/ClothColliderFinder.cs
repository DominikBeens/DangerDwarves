using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothColliderFinder : MonoBehaviour {

    public CapsuleCollider col1;
    public CapsuleCollider col2;
    public Cloth myCloth;

    private void Start() 
    {
        myCloth = GetComponent<Cloth>();

        col1 = transform.root.Find("CowlCollider1").GetComponent<CapsuleCollider>();
        col2 = transform.root.Find("CowlCollider2").GetComponent<CapsuleCollider>();

        myCloth.capsuleColliders[0] = col1;
        myCloth.capsuleColliders[1] = col2;
    }
}
