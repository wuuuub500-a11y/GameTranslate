using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private bool inBin = false;

    //  ±»À¬»øÍ°µ÷ÓÃ
    public void EnterBin()
    {
        if (inBin) return;

        inBin = true;
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
