using UnityEngine;
using System.Collections;

public class Enemy_Movement : MonoBehaviour {
	private enum States {
		Patrol, // Patrolling back and forth
		Alert, // A HA! A PLAYER!
		Chase, // Running at the player
		AttackStun, // Stunned back after ramming into player
		Death // Enemy is killed by player
	}

	public float walkSpeed = 3f;
	public float runSpeed = 6f;
	public Transform startPatrolPoint;
	public Transform endPatrolPoint;
	public float sightDistance = 6f;
	public float loseDistance = 15f;	// Distance enemy will lose interest when chasing the player

	private Transform myTrans;
	private NavMeshAgent agent;
	private States currState;
	private bool goingForwardInPath = true;	// If true, enemy is going from start to end points
	private float timer; // Used to keep track of alert/stun time

	void Start () {
		myTrans = transform;
		agent = GetComponent<NavMeshAgent>();
	}

	void ChangeState ( States newState ) {
		if ( newState == States.Patrol ) {
			agent.destination = endPatrolPoint.position;
			agent.speed = walkSpeed;
		}
		else if ( newState == States.Alert ) {
			timer = 0;
			agent.ResetPath();
			myTrans.LookAt( Player.Instance.playerPosition );
		}
		else if ( newState == States.Chase ) {
			agent.destination = Player.Instance.playerPosition;
			agent.speed = runSpeed;
		}

		currState = newState;
	}

	void Update () {
		switch ( currState ) {
			case States.Patrol:
				UpdatePatrol();
			break;

			case States.Alert:
				UpdateAlert();
			break;

			case States.Chase:
				UpdateChase();
			break;
		}
	}

	void UpdatePatrol () {
		// If at end of path, go back in opposite direction
		if ( !agent.hasPath ) {
			goingForwardInPath = !goingForwardInPath;
			if ( goingForwardInPath )
				agent.destination = endPatrolPoint.position;
			else
				agent.destination = startPatrolPoint.position;
		}

		// Draw a ray to the player and check angle/distance
		Vector3 playerPos = Player.Instance.playerPosition;
		if ( Vector3.Distance(myTrans.position,playerPos) <= sightDistance ) {
			RaycastHit hitInfo;
			if ( Physics.Linecast(myTrans.position, playerPos, out hitInfo) ) {
				if ( hitInfo.collider.gameObject.tag == "Player" ) {
					float angle = Vector3.Angle(playerPos-myTrans.position,myTrans.forward);
					// print("Angle: "+angle);
					if ( angle <= 65 )
						ChangeState( States.Alert );
				}
			}
		}
	}

	void UpdateAlert () {
		timer += Time.deltaTime;
		if ( timer >= 1.5f ) {
			ChangeState( States.Chase );
		}
	}

	void UpdateChase () {
		if ( Vector3.Distance(myTrans.position,Player.Instance.playerPosition) <= loseDistance ) {
			agent.destination = Player.Instance.playerPosition;
		}
		else {
			ChangeState( States.Patrol );
		}
	}
}
