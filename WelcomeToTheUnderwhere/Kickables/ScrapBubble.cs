using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A type of kickable that requires being kicked a certain cumulative distance before its shell will "crack."
// Once its shell is cracked, kicking it once more will add a new "scrap" (customization option) to the player's collection.
public class ScrapBubble : Kickable
{
	[Header("References")]
	public Renderer shellRenderer;
	public Image scrapImage;
	public ParticleSystem acquireParticles;

	[Header("Scrap Definition")]
	public ScrapDefinition scrapDefinition;
	public float particleDuration;

	[Header("Settings")]
	public float targetScale;
	public float scaleUnitPerDistanceUnit;
	public float crackedShellBufferTime;

	protected bool _hasAddedScrapToLibrary;
	protected bool _hasBeenKicked;
	protected bool _shellHasBeenCracked;
	protected bool _scrapAvailable;
	protected float _distanceTraveled;
	protected Vector3 _prevPos;

	void Start()
	{
		if (ScrapInventoryLibrary.s.ContainsScrapDefinition(scrapDefinition))
		{
			Debug.LogFormat("we already have scrap of sprite {0}. destroying!", scrapDefinition.scrapSprite.name);
			Destroy(gameObject);
		}
		else
		{
			SetupBubble();
			acquireParticles.gameObject.SetActive(false);
		}
	}

	void OnDestroy()
	{
		CancelInvoke();
	}

	void Update()
	{
		if (_hasBeenKicked && !_shellHasBeenCracked)
		{
			Vector3 delta = transform.position - _prevPos;
			float distance = delta.magnitude;
			_distanceTraveled += distance;
			_prevPos = transform.position;

			float newScale = 1 + (_distanceTraveled * scaleUnitPerDistanceUnit);
			transform.localScale = Vector3.one * newScale;

			if (newScale >= targetScale)
			{
				Debug.Log("crack shell!");
				CrackShell();
			}
		}
	}

	protected void CrackShell()
	{
		_shellHasBeenCracked = true;
		shellRenderer.enabled = false;
		acquireParticles.gameObject.SetActive(true);
		acquireParticles.transform.SetParent(null);
		acquireParticles.Play();
		Destroy(acquireParticles, particleDuration);
		Invoke("MakeScrapAvailable", crackedShellBufferTime);
	}

	protected void MakeScrapAvailable()
	{
		_scrapAvailable = true;
	}

	public void SetupBubble()
	{
		scrapImage.sprite = scrapDefinition.scrapSprite;

		float width = scrapDefinition.scrapSprite.textureRect.width;
		float length = scrapDefinition.scrapSprite.textureRect.height;
		float ratio = length / width;

		Vector2 sizeDelta = scrapImage.rectTransform.sizeDelta;

		if (length >= width)
		{
			sizeDelta.x = sizeDelta.x / ratio;
		}
		else
		{
			sizeDelta.y = sizeDelta.y * ratio;
		}

		scrapImage.rectTransform.sizeDelta = sizeDelta;
	}

	public override void Kick(Vector3 kickDir)
	{
		base.Kick(kickDir);

		if (!_hasBeenKicked)
		{
			_hasBeenKicked = true;
			_prevPos = transform.position;
		}
		else if (_shellHasBeenCracked && _scrapAvailable)
		{
			if (!_hasAddedScrapToLibrary)
			{
				Debug.Log("got it");
				_hasAddedScrapToLibrary = true;
				ScrapInventoryLibrary.s.AddScrapDefinition(scrapDefinition);
				Destroy(gameObject);
			}
		}
	}
}
