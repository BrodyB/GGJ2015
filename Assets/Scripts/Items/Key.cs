using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour {
	public enum KeyColor {
		Blue = 1,
		Orange = 2,
		Yellow = 3,
		Green = 4
	};

	public KeyColor keyColor;
	public Material orangeMaterial;
	public Material blueMaterial;
	public Material yellowMaterial;
	public Material greenMaterial;

	private Transform keyTrans;
	private Transform shineTrans;
	private float startY;

	void OnTriggerEnter ( Collider col ) {
		if ( col.gameObject.tag == "Player" ) {
			Player.Inventory.keys.Add( (int)keyColor );
			Destroy( gameObject );
		}
	}

	void Start () {
		keyTrans = transform.Find("key");
		startY = keyTrans.position.y;
		Material[] mats = keyTrans.renderer.materials;
		switch ( keyColor ) {
			case KeyColor.Blue:
				mats[1] = blueMaterial;
			break;

			case KeyColor.Orange:
				mats[1] = orangeMaterial;
			break;

			case KeyColor.Yellow:
				mats[1] = yellowMaterial;
			break;

			case KeyColor.Green:
				mats[1] = greenMaterial;
			break;
		}
		keyTrans.renderer.materials = mats;

		shineTrans = transform.Find("key_shine");
		iTween.ScaleTo( shineTrans.gameObject, iTween.Hash("y",1.5f, "time",1f, 
							"easetype",iTween.EaseType.easeInOutQuad,
							"looptype",iTween.LoopType.pingPong) );
	}
	
	void Update () {
		// Key updating
		keyTrans.position = new Vector3(keyTrans.position.x, 
							startY + (Mathf.Sin(Time.time) * 0.35f),
							keyTrans.position.z);
		keyTrans.Rotate( Vector3.up * 60 * Time.deltaTime );

		// Shine update
		shineTrans.Rotate( Vector3.up * -90 * Time.deltaTime );
	}
}
