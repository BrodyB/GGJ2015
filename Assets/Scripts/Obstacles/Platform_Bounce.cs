using UnityEngine;
using System.Collections;

public class Platform_Bounce : MonoBehaviour {
	public float bounceStrength = 4f;

	void OnTriggerEnter ( Collider col ) {
		if ( col.gameObject.tag == "Player" ) {
			col.transform.parent.GetComponent<PlayerMotor>().VerticalVelocity = bounceStrength;
		}
	}
}