using UnityEngine;
using System.Collections;

public class Decahedron : MonoBehaviour {
	private Transform decaTrans;
	private Transform shineTrans;
	private float startY;

	void OnTriggerEnter ( Collider col ) {
		if ( col.gameObject.tag == "Player" ) {
			Destroy( gameObject );
		}
	}

	void Start () {
		decaTrans = transform.Find("decahedron");
		startY = decaTrans.position.y;

		shineTrans = transform.Find("key_shine");
		iTween.ScaleTo( shineTrans.gameObject, iTween.Hash("y",1.5f, "time",1f, 
							"easetype",iTween.EaseType.easeInOutQuad,
							"looptype",iTween.LoopType.pingPong) );
	}
	
	void Update () {
		// Key updating
		decaTrans.position = new Vector3(decaTrans.position.x, 
							startY + (Mathf.Sin(Time.time) * 0.25f),
							decaTrans.position.z);
		decaTrans.Rotate( Vector3.up * 30 * Time.deltaTime );

		// Shine update
		shineTrans.Rotate( Vector3.up * -90 * Time.deltaTime );
	}
}
