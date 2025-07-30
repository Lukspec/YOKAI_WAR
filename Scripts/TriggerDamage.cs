using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDamage : MonoBehaviour
{
    public HeartSystem heart;
    public PlayerMov player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player.KBCount = player.KBTime;
            if(collision.transform.position.x <= transform.position.x)
            {
                player.isKnockRight = false;
            }
            if(collision.transform.position.x > transform.position.x)
            {
                player.isKnockRight = true;
            }
            heart.vida--;
            player.anim.SetTrigger("TakeDamage");
        }
    }
}
