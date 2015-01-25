using UnityEngine;
using System.Collections;

public class Platform_Bounce : MonoBehaviour {
	public float bounceStrength = 4f;

	void OnTriggerEnter ( Collider col ) {
		if ( col.gameObject.tag == "Player" ) {
			Player.Motor.VerticalVelocity = bounceStrength;
		}
	}
}