using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for objects that the player can "kick" (aka interact with and apply some force to)
// simply by walking into them.
public class Kickable : MonoBehaviour
{
	[Header("References")]
	public Rigidbody rigid;

	[Header("Settings")]
	public float horizontalMultiplier = 2f;
	public float verticalMultiplier = 0f;

	public virtual void Kick(Vector3 kickDir)
	{
		rigid.isKinematic = false;
		rigid.useGravity = true;
		rigid.velocity = new Vector3(kickDir.x * horizontalMultiplier, kickDir.y * verticalMultiplier, kickDir.z * horizontalMultiplier);
	}
}
