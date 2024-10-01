using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Life : MonoBehaviour
{
    [SerializeField] private AudioSource deathSoundEffect;
    private Rigidbody2D rb;
    private Animator anim;
    public Transform spawnPoint;  // players respawn point

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }
    }

    private void Die() 
    {
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("Death");
        deathSoundEffect.Play();
        gameObject.SetActive(false);  // Disable player object
        Invoke("Respawn", 2f);  // Respawn player after a delay
    }

    private void Respawn()
    {
        gameObject.SetActive(true);  // Re-enable player object
        transform.position = spawnPoint.position;  // Reset player position
        rb.bodyType = RigidbodyType2D.Dynamic;
        anim.ResetTrigger("Death");
        anim.Play("Idle"); 
    }
}

