using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Åöµ½ÁË: " + collision.name);

        if (collision.CompareTag("Projectile_Left"))
        {
            GameManager_2.instance.AddScoreLeft();
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Projectile_Right"))
        {
            GameManager_2.instance.AddScoreRight();
            Destroy(collision.gameObject);
        }
    }
}
