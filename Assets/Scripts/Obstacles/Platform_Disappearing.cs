using UnityEngine;
using System.Collections;

public class Platform_Disappearing : MonoBehaviour {
	public float activeTime = 2f;
	public float respawnTime = 3f;
	public float fadeTime = 0.7f;

	private int state = 0; // 0 = active, 1 = touched, 2 = fading, 3 = disappeared
	private float timer = 0;
	private Color startColor;

	void Start () {
		startColor = transform.parent.renderer.material.color;
	}

	void OnTriggerEnter ( Collider col ) {
		// print( "Tag: "+col.gameObject.tag+"   Name: "+col.gameObject.name );
		if ( col.gameObject.tag == "Player" ) {
			if ( state == 0 ) {
				ChangeState( 1 );
			}
		}
	}

	void ChangeState ( int newState ) {
		switch ( newState ) {
			case 0: // Active
				transform.parent.GetComponent<BoxCollider>().enabled = true;
				transform.parent.renderer.material.color = startColor;
			break;


			case 1: // Touched
				timer = 0;
			break;


			case 2: // Fading
				Hashtable fader = new Hashtable();
				fader.Add( "time", fadeTime );
				fader.Add( "a", 0 );
				fader.Add( "easetype", iTween.EaseType.linear );
				fader.Add( "oncompletetarget", gameObject );
				fader.Add( "oncomplete", "TurnOff" );
				iTween.ColorTo( transform.parent.gameObject, fader );
			break;


			case 3: // Disappeared
				transform.parent.GetComponent<BoxCollider>().enabled = false;
			break;
		}

		timer = 0;
		state = newState;
	}

	void Update () {
		timer += Time.deltaTime;
		if ( state == 1 ) {
			if ( timer > activeTime-fadeTime ) {
				ChangeState( 2 );
			}
		}
		else if ( state == 3 ) {
			if ( timer > respawnTime ) {
				ChangeState( 0 );
			}
		}
	}

	public void TurnOff () {
		ChangeState( 3 );
	}
}
