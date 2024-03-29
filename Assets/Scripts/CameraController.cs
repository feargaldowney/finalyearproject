using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]private Transform player;
    // A handy way of accessing the players transform is serializing the field and drag drop the player object into it.
    private void Update()
    {
        transform.position = new Vector3((player.position.x + 8), -0.13f, -25);
        // (player.position.y + 3.85f), - 25) old Y value for following players Y axius
    }
}
