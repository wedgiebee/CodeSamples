using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple code-only animation to make a transparent cube representing the ocean bob slowly up and down
public class Ocean : MonoBehaviour
{
	[Header("Settings")]
	public float amplitude;
	public float period;

	protected float _startY;

	void Start()
	{
		_startY = transform.position.y;
	}

	void Update ()
	{
		float sin = Mathf.Sin(Time.time / period);
		float adjustedSin = (sin + 1) / 2;
		float yValue = adjustedSin * amplitude;
		Vector3 pos = transform.position;
		pos.y = _startY + yValue;
		transform.position = pos;
	}
}
