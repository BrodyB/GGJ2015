using System;
using UnityEngine;
using UnityEngine.Internal;


public static class Helper
{
	public struct ClipPlanePoints
	{
		public Vector3 UpperLeft;
		public Vector3 UpperRight;
		public Vector3 LowerLeft;
		public Vector3 LowerRight;
	}
	
	public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos)
	{
		var clipPlanePoints = new ClipPlanePoints();
		
		if (Camera.main == null)
			return clipPlanePoints;
			
		var transform = Camera.main.transform;
		var halfFOV = Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad;
		var aspect = Camera.main.aspect;
		var distance = Camera.main.nearClipPlane;
		var height = distance * Mathf.Tan(halfFOV);
		var width = height * aspect;
		
		clipPlanePoints.UpperLeft = pos - transform.right * width + transform.up * height + transform.forward * distance;
		clipPlanePoints.UpperRight = pos + transform.right * width + transform.up * height + transform.forward * distance;
		clipPlanePoints.LowerLeft = pos - transform.right * width - transform.up * height + transform.forward * distance;
		clipPlanePoints.LowerRight = pos + transform.right * width - transform.up * height + transform.forward * distance;
		
		return clipPlanePoints;
	}
	
	public static float WrapAngle(float angle)
	{
		//If its negative rotate until its positive
		while (angle < 0)
			angle += 360;
		
		//If its to positive rotate until within range
		return Mathf.Repeat(angle, 360);
	}

	public static float ClampAngle(float angle, float min = 0, float max = 360)
	{
		//WrapAngle(angle);
		
		return Mathf.Clamp(angle, min, max);
	}
	
	public static Vector3 SuperSmoothLerp(Vector3 x0, ref Vector3 y0, Vector3 yt, float t)
	{
		
		Vector3 num = yt - (yt - y0) / t + (x0 - y0 + (yt - y0) / t) * Mathf.Exp(-t);
		y0 = yt;
		return num;
	}
	
	
	public static float SuperSmoothLerpAngle(float x0, ref float y0, float yt, float t)
	{
		float num = Mathf.Repeat (y0 - x0, 360f);
		float num2 = Mathf.Repeat (yt - x0, 360f);
		if (num > 180f)
		{
			num -= 360f;
		}
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		y0 = x0 + num;
		yt = x0 + num2;
		float num3 = WrapAngle(yt - (yt - y0) / t + (x0 - y0 + (yt - y0) / t) * Mathf.Exp(-t));
		y0 = yt;
		return num3;
	}
	
	public static float SuperSmoothLerp(float x0, float y0, float yt, float t, float k)
	{
		float f = x0 - y0 + (yt - y0) / (k * t);
		return yt - (yt - y0) / (k*t) + f * Mathf.Exp(-k*t);
	}


	public static float CheckCameraPoints(Camera cam, Vector3 from, Vector3 to)
	{
		float nearestDistance = -1f;
		
		RaycastHit hitInfo;
		
		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);
		
		//		Debug.DrawLine(from, to + transform.forward * -camera.nearClipPlane, Color.red);
		//		Debug.DrawLine(from, clipPlanePoints.UpperLeft);
		//		Debug.DrawLine(from, clipPlanePoints.UpperRight);
		//		Debug.DrawLine(from, clipPlanePoints.LowerLeft);
		//		Debug.DrawLine(from, clipPlanePoints.LowerRight);
		//		
		//		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
		//		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
		//		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
		//		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);
		
		if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo, ~Layers.dynamic) && hitInfo.collider.tag != "Player")
			nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo, ~Layers.dynamic) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo, ~Layers.dynamic) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo, ~Layers.dynamic) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, to + cam.transform.forward * -cam.nearClipPlane, out hitInfo, ~Layers.dynamic) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		return nearestDistance;
	}
}

