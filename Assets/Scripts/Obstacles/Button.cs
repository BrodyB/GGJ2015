using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
	public GameObject target;
	public bool canOnlyBePressedOnce = false;
	public bool requiresKey = false;
	public Key.KeyColor keyColorRequired;

	private bool isActive = true;

	void Start () {
		if (target == null)
			isActive = false;
	}

	void OnTriggerEnter ( Collider col ) {
		if ( isActive && col.gameObject.tag == "Player" ) {
			if ( !requiresKey || (requiresKey && Player.Inventory.keys.Contains((int)keyColorRequired)) ) {
				print("Activate");
				target.SendMessage( "OnActivate", SendMessageOptions.DontRequireReceiver );
			}
		}
	}

	void OnTriggerExit ( Collider col ) {
		if ( isActive && col.gameObject.tag == "Player" ) {
			if ( !requiresKey || (requiresKey && Player.Inventory.keys.Contains((int)keyColorRequired)) ) {
				target.SendMessage( "OnDeactivate", SendMessageOptions.DontRequireReceiver );
				if (canOnlyBePressedOnce)
					isActive = false;
			}
		}
	}
}