using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]private Transform player;
    private void Update()
    {
        // camera follows player
        transform.position = new Vector3((player.position.x + 8), -0.13f, -25);
    }
}
