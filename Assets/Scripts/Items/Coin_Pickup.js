private var model : Transform;

function Awake () {
	model = transform.Find("mdl_coin");
}

function OnTriggerEnter () {
	audio.Play();
	CollectCoin();
}

function CollectCoin () {
	rigidbody.isKinematic = false;
	rigidbody.useGravity = true;
	rigidbody.AddForce (Vector3.up * 200);
	rigidbody.AddTorque (Random.Range(0,50),0,Random.Range(0,50));
	
	yield WaitForSeconds(0.7);
	
	Destroy(gameObject);
}


function Update () {
	model.localPosition.y = 1 + Mathf.PingPong(Time.time * 0.5,0.5);
	model.Rotate(Vector3.up * 110 * Time.deltaTime);
}