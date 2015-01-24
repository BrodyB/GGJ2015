using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour {

    public Vector2 leftAnalog { get; set; }
    public Vector2 rightAnalog { get; set; }

    private bool jump;
    private bool endJump;
    private bool activate;
    private bool drop;
    private bool attack;

    private GameObject weapon;
    private GameObject gun;

    private Dictionary<Action, float> actionQueue = new Dictionary<Action, float>()
    {
        {Action.Jump, 0f},
    };

    public enum Action
	{
		None     = 0,
        Jump     = 1 << 0,
        EndJump  = 1 << 1,
        Activate = 1 << 2,
        Release  = 1 << 3,
	}

    void Awake()
    {
        //weapon = transform.FindChild("weapon").gameObject;
        //gun = transform.FindChild("gun").gameObject;
    }

	public void ProcessInput()
	{
        leftAnalog = Vector2.zero;
        rightAnalog = Vector2.zero;
	
		//if (!GameController.Instance.planningMode && Input.GetAxis("Right Trigger") < 0.9f)
        {
            leftAnalog = GetLocomotionInput(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            rightAnalog = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            if (Input.GetButtonDown("Jump")) actionQueue[Action.Jump] = 0.1f;

            jump = actionQueue[Action.Jump] > 0f;
            endJump = !Input.GetButton("Jump");
            activate = Input.GetButtonDown("Activate");
            drop = Input.GetButtonDown("Right Bumper");
            attack = Input.GetButtonDown("Cancel");
        }
        
		HandleActionInput();
        UpdateTimedInputs();
	}

    Vector2 GetLocomotionInput(float horizontal, float vertical)
	{
		float sqrMagnitude = horizontal * horizontal + vertical * vertical;
        Vector2 inputVector = Vector3.zero;

        if (sqrMagnitude > 0.01f)
		{
            float sqrVertical = vertical < 0 ? - vertical * vertical : vertical * vertical;
            float sqrHorizontal = horizontal < 0 ? - horizontal * horizontal : horizontal * horizontal;

            inputVector += new Vector2(0, sqrVertical);
            inputVector += new Vector2(sqrHorizontal, 0);
		}

        return Vector2.ClampMagnitude(inputVector, 1f);
	}
	
	void HandleActionInput()
	{
        if (jump)
			Player.Motor.TryJump();

        if (endJump)
            Player.Motor.EndJump();

        //if (activate)
        //{
        //    ActivatableObject target = GameController.Instance.player.closestActivatableObject;
        //    if (target != null)
        //    {
        //        target.Activate();
        //        if (target is IAcquirable)
        //           Player.Inventory.Acquire(target.gameObject);
        //    }
        //    else
        //    {
        //        Debug.Log("Nothing to activate...");
        //    }
        //}

        if (drop)
            Player.Inventory.Drop();

        if (attack)
            StartCoroutine(DoAttack());
	}
	
    IEnumerator DoAttack()
    {
        //if (gun == null || !gun.activeInHierarchy)
        //{
        //    float i = 0f;
        //    float time = 0.2f;

        //    weapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        //    weapon.transform.GetChild(0).gameObject.SetActive(true);

        //    while (i <= time)
        //    {
        //        i += Time.deltaTime;
        //        float rotation = Mathf.Lerp(0f, 90f, i / time);
        //        weapon.transform.localRotation = Quaternion.Euler(new Vector3(0f, rotation, 0f));
        //        yield return 0;
        //    }
        //    weapon.transform.GetChild(0).gameObject.SetActive(false);
        //}
        //else
        //{
        //    ProjectileLauncher pl = gun.GetComponent<ProjectileLauncher>();
        //    pl.createAndFire();
        //}
        yield return 0;

    }

	void UpdateTimedInputs()
	{
		var list = new List<Action>(actionQueue.Keys);
		
		foreach(var action in list)
		{
			if (actionQueue[action] > 0f)
				actionQueue[action] -= Time.deltaTime;
			if (actionQueue[action] < 0f)
				actionQueue[action] = 0f;
		}
	}
}
