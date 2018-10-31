using UnityEngine;
using Y3P1;

public class ClothColliderFinder : MonoBehaviour
{

    public Cloth myCloth;

    private void Start()
    {
        CapsuleCollider[] cols = new CapsuleCollider[]
            {
                Player.localPlayer.collider1,
                Player.localPlayer.collider2
            };

        myCloth.capsuleColliders = cols;
    }
}
