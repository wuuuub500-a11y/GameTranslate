using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour
{
    public int scoreLeft = 0;
    public int scoreRight = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile_Left"))
        {
            scoreLeft++;
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Projectile_Right"))
        {
            scoreRight++;
            Destroy(collision.gameObject);
        }
    }
}
