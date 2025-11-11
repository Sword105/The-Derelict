using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SuspiciousNodeData : MonoBehaviour
{
    // ONLY assign this value if, when a player interacts with the object, it implies the immediate suspicion of another
    public Transform impliedObject;
}
