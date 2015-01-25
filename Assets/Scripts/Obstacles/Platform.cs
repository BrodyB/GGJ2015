using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
    private bool isPlayerAbove = false;
    protected bool isPlayerOnPlatform = false;

    void Update()
    {
        isPlayerOnPlatform = isPlayerAbove && Player.Motor.isGrounded;
        OnUpdate();
    }

    public virtual void OnUpdate()
    {
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == Tags.player) isPlayerAbove = true;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == Tags.player) isPlayerAbove = false;
    }
}
