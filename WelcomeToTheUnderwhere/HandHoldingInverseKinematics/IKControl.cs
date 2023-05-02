using UnityEngine;
using System;
using System.Collections;

//  Apply Inverse Kinematics to a character, so that he reaches for the other character.
// Determines whether it makes sense to extend the left or right hand depending on how the characters
// are standing relative to each other.
public class IKControl : MonoBehaviour
{
    public enum WhichHand
    {
        Left,
        Right
    }

    [Header("References")]
    public Animator animator;
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftSideReference;
    public Transform rightSideReference;

    [Header("Settings")]
    public bool ikActive = false;
    public WhichHand whichHand;
    public int playerIndex;

    protected Transform _targetObj = null;
    protected Transform _lookObj = null;
    protected HandHoldingTarget _handHoldingTarget;
    protected float _weight;

    public void TurnOffIK()
    {
        ikActive = false;
        Debug.Log("turned off");
    }

    public void SetIKWeight(float weight)
    {
        _weight = weight;
    }

    public void TurnOnIK(HandHoldingTarget handHoldingTarget = null)
    {
        ikActive = true;

        if (handHoldingTarget != null)
        {
            _handHoldingTarget = handHoldingTarget;
            _targetObj = handHoldingTarget.transform;
            _lookObj = handHoldingTarget.transform;
        }
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {

                // Set the look target position, if one has been assigned
                if (_lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(_lookObj.position);
                }    

                // Set the right hand target position and rotation, if one has been assigned
                if (_targetObj != null)
                {
                    DecideWhichHandToUse();

                    AvatarIKGoal ikGoal = GetCorrectIKGoalForHand(whichHand);

                    animator.SetIKPositionWeight(ikGoal, _weight);
                    animator.SetIKRotationWeight(ikGoal, _weight);  
                    animator.SetIKPosition(ikGoal, _targetObj.position);
                    //animator.SetIKRotation(ikGoal, targetObj.rotation);
                    animator.SetIKRotation(ikGoal, GetLookAtRotation());

                    AvatarIKGoal oppositeIKGoal = GetOppositeIKGoalForHand(whichHand);

                    // these 2 lines don't seem to be needed at all! maybe i am misunderstanding how ik works?
                    animator.SetIKPositionWeight(oppositeIKGoal, 0);
                    animator.SetIKRotationWeight(oppositeIKGoal, 0); 
                }        

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0); 
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0); 
                animator.SetLookAtWeight(0);
            }
        }
    }

    protected AvatarIKGoal GetCorrectIKGoalForHand(WhichHand hand)
    {
        if (hand == WhichHand.Left)
        {
            return AvatarIKGoal.LeftHand;
        }
        else
        {
            return AvatarIKGoal.RightHand;
        }
    }

    protected AvatarIKGoal GetOppositeIKGoalForHand(WhichHand hand)
    {
        if (hand == WhichHand.Left)
        {
            return AvatarIKGoal.RightHand;
        }
        else
        {
            return AvatarIKGoal.LeftHand;
        }
    }

    protected void DecideWhichHandToUse()
    {
        float leftDistance = Vector3.Distance(leftSideReference.position, _targetObj.position);
        float rightDistance = Vector3.Distance(rightSideReference.position, _targetObj.position);

        if (leftDistance <= rightDistance)
        {
            whichHand = WhichHand.Left;
        }
        else
        {
            whichHand = WhichHand.Right;
        }
    }

    protected Quaternion GetLookAtRotation()
    {
        _targetObj.LookAt(_handHoldingTarget.GetOtherPlayerTransform(playerIndex).position);
        return _targetObj.rotation;
    }
}