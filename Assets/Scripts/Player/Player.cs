using UnityEngine;
using System.Collections;

public class Player : Singleton<Player>
{
    //public ActivatableObject closestActivatableObject;
    //public int playerSecurityClearance = 0;
    //public float closestActivatableObjectDistance { get; set; }

    public static PlayerMotor Motor;
    public static PlayerInput Input;
    public static PlayerInventory Inventory;
    public static PlayerAnimator Animator;
    //public Canvas UI;

    //public Vector3[] playerVertices;

    
    private GameObject player;
    private GameObject playerMesh;
    private Mesh playerMeshFilter;
    private Vector3 playerMeshScale;

    public Vector3 playerPosition { get { return transform.position; } }

    void Awake()
    {
        Motor = GetComponent<PlayerMotor>();
        Input = GetComponent<PlayerInput>();
        Inventory = GetComponent<PlayerInventory>();
        Animator = GetComponent<PlayerAnimator>();

        //UI = FindObjectOfType<Canvas>();

        //player = GameObject.FindGameObjectWithTag(Tags.player);
        //playerMesh = GameObject.FindGameObjectWithTag(Tags.playerMesh);
        //playerMeshFilter = playerMesh.GetComponent<MeshFilter>().sharedMesh;

        //playerVertices = playerMeshFilter.vertices;
    }

    void Update()
    {
        //playerVertices = playerMeshFilter.vertices;
        //if (!disabled)
        {
            Input.ProcessInput();
            Motor.ProcessMotion();
            Animator.ProcessAnimation();
        }

        //playerMesh.renderer.enabled = !disabled;
    }
}
