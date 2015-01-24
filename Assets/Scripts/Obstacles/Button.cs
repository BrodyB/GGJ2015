using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
	public GameObject target;
	public bool canOnlyBePressedOnce = false;

	private bool isActive = true;

	void Start () {
		if (target == null)
			isActive = false;
	}

	void OnTriggerEnter ( Collider col ) {
		if ( isActive && col.gameObject.tag == "Player" ) {
			print("Activate");
			target.SendMessage( "OnActivate", SendMessageOptions.DontRequireReceiver );
		}
	}

	void OnTriggerExit ( Collider col ) {
		if ( isActive && col.gameObject.tag == "Player" ) {
			target.SendMessage( "OnDeactivate", SendMessageOptions.DontRequireReceiver );
			if (canOnlyBePressedOnce)
				isActive = false;
		}
	}
}
