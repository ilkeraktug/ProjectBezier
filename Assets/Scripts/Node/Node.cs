using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, 0.4f);
    }
}
