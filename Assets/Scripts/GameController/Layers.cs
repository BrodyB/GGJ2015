using UnityEngine;
using System.Collections;

public class Layers : MonoBehaviour
{
    public static LayerMask player = 1 << LayerMask.NameToLayer("Player");
    public static LayerMask enemies = 1 << LayerMask.NameToLayer("Enemies");
    public static LayerMask dynamic = 1 << LayerMask.NameToLayer("Dynamic");
}
