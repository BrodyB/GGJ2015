using UnityEngine;
using System.Collections;

public class GameController : Singleton<GameController>
{
	
	public GameObject player;

    //public GameObject
	
	// Use this for initialization
	void Awake ()
	{
        player = GameObject.FindGameObjectWithTag(Tags.player);

        //PlayerMotor = FindObjectOfType(typeof(PlayerMotor)) as PlayerMotor;
        //PlayerInput = FindObjectOfType(typeof(PlayerInput)) as PlayerInput;
        //PlayerAnimator = FindObjectOfType(typeof(PlayerAnimator)) as PlayerAnimator;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
