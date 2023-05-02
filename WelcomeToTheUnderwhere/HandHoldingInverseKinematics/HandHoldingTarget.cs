using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Calculate where an IK target should be placed between two humanoid characters to make it look like they're holding hands.
// The height of the target is relative to their distance to each other, just like real hand-holding.
// (Closer = IK target closer to the ground, Further = IK target higher from the ground so it looks like they're reaching for each other.)
public class HandHoldingTarget : MonoBehaviour
{
	[Header("Settings")]
	public bool onlyHoldHandsIfStanding;
	public float maxHandHoldingDistance;
	public float distanceD;
	public float heightA;
	public float heightB;
	public float distanceA;
	public float distanceB;
	public float sittingOffsetHeight;
	public float lyingDownOffsetHeight;

	protected Underwearer _underwearer1;
	protected Underwearer _underwearer2;

	void Update ()
	{
		if (_underwearer1 != null && _underwearer2 != null)
		{
			if (ShouldHoldHands())
			{
				float distance = Vector3.Distance(_underwearer1.transform.position, _underwearer2.transform.position);
				float t = Mathf.InverseLerp(distanceD, maxHandHoldingDistance, distance);

				_underwearer1.ikControl.SetIKWeight(t);
				_underwearer2.ikControl.SetIKWeight(t);

				Vector3 pos = transform.position;
				float originalY = pos.y;
				pos = (_underwearer1.transform.position + _underwearer2.transform.position) / 2;
				pos.y = CalculateY(distance);
				transform.position = pos;
			}
			else
			{
				_underwearer1.ikControl.SetIKWeight(0);
				_underwearer2.ikControl.SetIKWeight(0);
			}
		}
		else
		{
			Debug.Log("can't calculate handholding target! one of the player refs is not assigned!");
		}
	}

	protected bool ShouldHoldHands()
	{
		if (onlyHoldHandsIfStanding)
		{
			return (_underwearer1.underwearerAnimator.heightState == UnderwearerAnimator.HeightState.Standing && _underwearer2.underwearerAnimator.heightState == UnderwearerAnimator.HeightState.Standing);
		}
		else
		{
			return true;
		}
	}

	protected float CalculateY(float distance)
	{
		float average = (GetHeight(_underwearer1) + GetHeight(_underwearer2)) / 2;
		float t = Mathf.InverseLerp(distanceA, distanceB, distance);
		return (average + Mathf.Lerp(heightA, heightB, t));
	}

	protected float GetHeight(Underwearer underwearer)
	{
		if (underwearer.underwearerAnimator.heightState == UnderwearerAnimator.HeightState.Sitting)
		{
			return underwearer.transform.position.y + sittingOffsetHeight;
		}
		else if (underwearer.underwearerAnimator.heightState == UnderwearerAnimator.HeightState.Lying)
		{
			return underwearer.transform.position.y + lyingDownOffsetHeight;
		}
		else
		{
			return underwearer.transform.position.y;
		}
	}

	public void SetupHandholding(Underwearer underwearer1, Underwearer underwearer2)
	{
		_underwearer1 = underwearer1;
		_underwearer2 = underwearer2;

		_underwearer1.ikControl.TurnOnIK(this);
		_underwearer2.ikControl.TurnOnIK(this);
	}

	public Transform GetOtherPlayerTransform(int playerIndex)
	{
		if (playerIndex <= 0)
		{
			return _underwearer2.transform;
		}
		else
		{
			return _underwearer1.transform;
		}
	}

}
