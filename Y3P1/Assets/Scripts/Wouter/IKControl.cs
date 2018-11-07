using Photon.Pun;
using UnityEngine;
using Y3P1;

[RequireComponent(typeof(Animator))]
public class IKControl : MonoBehaviour, IPunObservable
{

    protected Animator animator;

    public bool ikActive = false;
    public bool extrabool = true;
    public Transform rightHandObj = null;
    public Transform rightHandObjRot;
    public Transform leftHandObj = null;
    public Transform leftHandObjRot;
    public Transform lookObj;

    public bool trackPlayer;

    private void Awake()
    {
        if(trackPlayer)
        {
            //lookObj = Player.localPlayer.transform;
        }
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //lookObj = Player.localPlayer.transform;     
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive && extrabool)
            {
                // Set the look target position, if one has been assigned
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    //animator.SetIKPosition(AvatarIKGoal.RightHand, PlayerController.mouseInWorldPos);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObjRot .rotation);
                }
                if(leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    //animator.SetIKPosition(AvatarIKGoal.RightHand, PlayerController.mouseInWorldPos);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObjRot.rotation);
                }

            }
            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ikActive);
            stream.SendNext(enabled);
        }
        else
        {
            ikActive = (bool)stream.ReceiveNext();
            enabled = (bool)stream.ReceiveNext();
        }
    }
    
    void Update()
    {
        
    }
}