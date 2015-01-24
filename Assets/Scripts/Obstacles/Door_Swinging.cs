using UnityEngine;
using System.Collections;

public class Door_Swinging : MonoBehaviour {
	public GameObject knobObject;
	public float openAngle = 85f;
	public float openTime = 1f;
	public bool startsOpen = false;
	public AudioClip swingOpenSound;
	public AudioClip swingCloseSound;
	public AudioClip closeSound;
	
	private float startRotation;
	private bool canInteract = true;
	private bool isOpen = false;
	
	void Start () {
		startRotation = transform.eulerAngles.y;

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
			Hashtable doorTween = new Hashtable();
			doorTween.Add ("easetype",iTween.EaseType.easeOutExpo);
			doorTween.Add ("time",openTime);
			doorTween.Add ("islocal",true);
			doorTween.Add ("oncomplete","OnDoorFinishedOpening");

			Hashtable knobTween = new Hashtable();
			knobTween.Add("name","knob");
			knobTween.Add("islocal",true);

			isOpen = !isOpen;

			if (isOpen) { // Swing door open
				doorTween.Add ("y",startRotation+openAngle);
				doorTween.Add ("delay",0.2f);

				knobTween.Add ("easetype",iTween.EaseType.easeOutCirc);
				knobTween.Add ("time",0.3f);
				knobTween.Add ("x",-45f);

				PlayAudio(swingOpenSound);
			}
			else { // Swing door closed
				doorTween.Add ("y",startRotation);

				knobTween.Add ("easetype",iTween.EaseType.easeOutCirc);
				knobTween.Add ("time",0.2f);
				knobTween.Add ("x",-20f);

				PlayAudio(swingCloseSound);
			}
				
			iTween.RotateTo(gameObject,doorTween);

			if (knobObject != null) { iTween.RotateTo (knobObject,knobTween); }
			canInteract = false;
		}
	}

	void OnDoorFinishedOpening () {
		if (!isOpen) { PlayAudio (closeSound); }

		// Animate the door knob going back to neutral position
		Hashtable knobTween = new Hashtable();
		knobTween.Add("name","knob");
		knobTween.Add("islocal",true);
		knobTween.Add ("easetype",iTween.EaseType.easeOutElastic);
		knobTween.Add ("time",0.5f);
		knobTween.Add ("x",0f);
		if (knobObject != null) { iTween.RotateTo(knobObject,knobTween); }

		canInteract = true;
	}

	void SetDoorState (bool setOpen, bool instantly = true) {
		if (instantly) {
			isOpen = setOpen;
			Vector3 newRot = Vector3.zero;
			if (isOpen)
				newRot.y = startRotation+openAngle;
			else
				newRot.y = startRotation;

			transform.eulerAngles = newRot;
		}
		else {
			Hashtable tween = new Hashtable();
			tween.Add ("easetype",iTween.EaseType.easeOutQuad);
			tween.Add ("time",openTime);
			tween.Add ("oncomplete","OnDoorFinishedOpening");

			isOpen = setOpen;
			if (setOpen)
				tween.Add ("y",startRotation+openAngle);
			else
				tween.Add ("y",0f);

			iTween.RotateTo(gameObject,tween);
			canInteract = false;
		}
	}

	void PlayAudio ( AudioClip clip ) {
		if (audio && clip) {
			audio.Stop();
			audio.clip = clip;
			audio.Play();
		}
	}
}
