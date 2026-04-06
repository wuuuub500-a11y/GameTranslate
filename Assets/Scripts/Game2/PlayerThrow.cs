using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform throwPoint;

    public float minForce = 5f;
    public float maxForce = 15f;
    public float chargeTimeMax = 2f;

    public float throwAngle = 45f; // 固定角度

    public KeyCode throwKey;

    private float chargeTimer;
    private bool isCharging;

    void Update()
    {
        // 左玩家：A键
        if (Input.GetKeyDown(throwKey))
        {
            StartCharge();
        }

        if (Input.GetKey(throwKey))
        {
            Charging();
        }

        if (Input.GetKeyUp(throwKey))
        {
            Throw();
        }
    }

    void StartCharge()
    {
        isCharging = true;
        chargeTimer = 0;
    }

    void Charging()
    {
        if (!isCharging) return;

        chargeTimer += Time.deltaTime;
        chargeTimer = Mathf.Clamp(chargeTimer, 0, chargeTimeMax);
    }

    void Throw()
    {
        if (!isCharging) return;

        isCharging = false;

        float t = chargeTimer / chargeTimeMax;
        float force = Mathf.Lerp(minForce, maxForce, t);

        GameObject obj = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

        // 计算固定角度方向
        float rad = throwAngle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        rb.AddForce(dir * force, ForceMode2D.Impulse);
    }
}
