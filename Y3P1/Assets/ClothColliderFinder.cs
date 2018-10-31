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

        if (myCloth.capsuleColliders[0] != null && myCloth.capsuleColliders[1] != null)
        {
            Debug.LogWarning("Gottem");
        }
    }
}
