using UnityEngine;
using System.Collections;

public class Door_Sliding : MonoBehaviour {
	public float openDistance = 1.5f;
	public float openTime = 1f;
	public bool startsOpen = false;
	public AudioClip slideOpenSound;
	public AudioClip slideCloseSound;
	public AudioClip closeSound;
	
	private float startPosition;
	private bool canInteract = true;
	private bool isOpen = false;
	
	void Start () {
		startPosition = transform.localPosition.x;

		if (startsOpen)
			SetDoorState (true);

		if (audio == null) {
			gameObject.AddComponent<AudioSource>();
		}
	}

	void OnActivate () {
		ToggleDoor();
	}

	void ToggleDoor (bool instantly = false) {
		if (canInteract) {
			isOpen = !isOpen;
			float newPosition;
			if (isOpen) {
				newPosition = startPosition+openDistance;
				PlayAudio(slideOpenSound);
			}
			else {
				newPosition = startPosition;
				PlayAudio(slideCloseSound);
			}
			
			Hashtable tween = new Hashtable();
			tween.Add ("easetype",iTween.EaseType.easeOutExpo);
			tween.Add ("time",openTime);
			tween.Add ("islocal",true);
			tween.Add ("oncomplete","OnDoorFinishedOpening");
			tween.Add ("x",newPosition);

			iTween.MoveTo(gameObject,tween);
			canInteract = false;
		}
	}

	void OnDoorFinishedOpening () {
		if (!isOpen) { PlayAudio (closeSound); }
		canInteract = true;
	}

	void SetDoorState (bool setOpen) {
		isOpen = setOpen;
		Vector3 newPos = Vector3.zero;
		if (isOpen)
			newPos.x = startPosition+openDistance;
		else
			newPos.x = startPosition;

		transform.localPosition = newPos;
	}

	void PlayAudio ( AudioClip clip ) {
		if (audio && clip) {
			audio.Stop();
			audio.clip = clip;
			audio.Play();
		}
	}
}
