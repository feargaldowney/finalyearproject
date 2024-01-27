using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    
    private int cherries = 0;
    [SerializeField]private TMP_Text cherriesText;
    [SerializeField]private AudioSource collectSoundEffect;
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Cherry"))
        {
            Destroy(collision.gameObject);
            cherries++;
            collectSoundEffect.Play();
            cherriesText.text = "Cherries: " + cherries;
        }
    }
}
