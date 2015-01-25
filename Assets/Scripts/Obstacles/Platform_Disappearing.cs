using UnityEngine;
using System.Collections;

public class Platform_Disappearing : MonoBehaviour
{
	public float activeTime = 2f;
	public float respawnTime = 3f;
	public float fadeTime = 0.7f;

	private int state = 0; // 0 = active, 1 = touched, 2 = fading, 3 = disappeared
	private float timer = 0;
	private Color startColor;

    private bool isPlayerAbove = false;

    private Collider phyicalCollider;

    void Awake()
    {
        phyicalCollider = GetComponents<BoxCollider>()[0];
        startColor = renderer.material.color;
    }

    void OnTriggerEnter ( Collider col )
    {
        if (col.gameObject.tag == "Player") isPlayerAbove = true;
	}

    void OnTriggerExit( Collider col )
    {
        if (col.gameObject.tag == "Player") isPlayerAbove = false;
    }

	void ChangeState ( int newState )
    {
		switch ( newState )
        {
			case 0: // Active
                phyicalCollider.enabled = true;
				renderer.material.color = startColor;
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
				iTween.ColorTo( gameObject, fader );
			break;

            case 3: // Disappeared
                phyicalCollider.enabled = false;
			break;
		}

		timer = 0;
		state = newState;
	}

	void Update () {

		timer += Time.deltaTime;

        if ( state == 0 )
        {
            if (isPlayerAbove && Player.Motor.isGrounded) ChangeState(1);
        }
		else if ( state == 1 )
        {
			if ( timer > activeTime-fadeTime ) ChangeState( 2 );
		}
		else if ( state == 3 )
        {
			if ( timer > respawnTime ) ChangeState( 0 );
		}

	}

	public void TurnOff () {
		ChangeState( 3 );
	}
}
