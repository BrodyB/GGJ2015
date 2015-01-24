using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
	public static PlayerAnimator Instance;
	//public Direction MoveDirection { get; set; }
	
	public enum Direction
	{
		Idle = 0,
		North = 1,
		South = 2,
		East = 3,
		NorthEast = 4,
		SouthEast = 5,
		West = 6,
		NorthWest =7,
		SouthWest = 8,
	}
	
	public Direction DefaultDirection = Direction.South;
	public bool FlipLeftEnabled = false;
	
	private int? lastDir;
	//private Animator animator;
	private float threshold;
	
	void Awake()
	{
		//animator = this.GetComponent<Animator>();
		//threshold = Mathf.Sin(22.5f * Mathf.Deg2Rad); //45 deg = 4 sprites, 22.5 deg = 8 sprites
	}
	
	public void ProcessAnimation()
	{
		int dir = lastDir.HasValue ? (int)Player.Motor.MoveDirection
								   : (int)DefaultDirection;
		
		if (dir > 0)
		{
			//Coerce diagonals to 4-directional sprites
			//dir = FixDiagonals(dir);
			
			Vector2 dirVector = DirectionToVector2(dir);
//			animator.SetInteger("Direction", dir);
//			animator.SetFloat("DirectionX", dirVector.x);
//			animator.SetFloat("DirectionY", dirVector.y);
//			animator.SetBool("Moving", true);
			
			if (FlipLeftEnabled && dirVector.x != 0)
				transform.localScale = new Vector3(dirVector.x, 1, 1);
		}
		//else if (lastDir > 0) animator.SetBool("Moving", false);
		
		//animator.SetFloat("VerticalVelocity", GameController.PlayerMotor.VerticalVelocity);
		//animator.SetBool("isGrounded", TP_Controller.CharacterController.isGrounded);
		
		//if ((int)GameController.PlayerMotor.CurrentAction != 0)
			//animator.SetTrigger(GameController.PlayerMotor.CurrentAction.ToString());
		
		lastDir = dir;
	}
	
	private Vector2 DirectionToVector2(int dir)
	{
		int y = dir % 3;
		int x = dir / 3;
		if (y == 2) y = -1;
		if (x == 2) x = -1;
		return new Vector2(x,y);
	}
	
	private int FixDiagonals(int dir)
	{
		if (dir == (int)Direction.NorthEast ||
		    dir == (int)Direction.SouthEast)
		{
			if (lastDir == (int)Direction.North ||
			    lastDir == (int)Direction.South)
				dir -= (int)Direction.East;
			else dir = (int)Direction.East;
		}
		if (dir == (int)Direction.NorthWest ||
		    dir == (int)Direction.SouthWest)
		{
			if (lastDir == (int)Direction.North ||
			    lastDir == (int)Direction.South)
				dir -= (int)Direction.West;
			else dir = (int)Direction.West;
		}
		return dir;
	}
}
