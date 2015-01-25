using UnityEngine;
using System.Collections;

public class Platform_Bounce : Platform
{
	public float bounceStrength = 4f;

    public override void OnUpdate()
    {
        if (isPlayerOnPlatform) Player.Motor.VerticalVelocity = bounceStrength;
    }
}