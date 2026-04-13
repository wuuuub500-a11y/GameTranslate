using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrow_R : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform throwPoint;

    public KeyCode throwKey = KeyCode.L;

    public float minForce = 5f;
    public float maxForce = 15f;
    public float chargeTimeMax = 2f;

    public float throwAngle = 135f; //  往左上

    private float chargeTimer;
    private bool isCharging;

    private GameObject heldObject;
    private Rigidbody2D heldRb;

    private AudioSource audioSource;

    private bool canSpawn = true; // 控制生成节奏

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!GameManager_2.instance.CanThrowRight())
            return;

        // ?没有手持物体 && 允许生成 → 才生成
        if (heldObject == null && canSpawn)
        {
            SpawnHeldObject();
        }

        // 没有物体就不执行输入
        if (heldObject == null)
            return;

        // 开始蓄力
        if (Input.GetKeyDown(throwKey))
        {
            isCharging = true;
            chargeTimer = 0f;
        }

        // 蓄力中
        if (Input.GetKey(throwKey))
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Clamp(chargeTimer, 0f, chargeTimeMax);
        }

        // 松手投掷
        if (Input.GetKeyUp(throwKey))
        {
            Throw();
        }
    }

    void SpawnHeldObject()
    {
        heldObject = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        heldRb = heldObject.GetComponent<Rigidbody2D>();

        // ?冻结在手上
        heldRb.velocity = Vector2.zero;
        heldRb.angularVelocity = 0f;
        heldRb.bodyType = RigidbodyType2D.Kinematic;

        heldObject.transform.position = throwPoint.position;
        heldObject.transform.SetParent(throwPoint);
    }

    void Throw()
    {
        isCharging = false;

        if (heldObject == null) return;

        heldObject.transform.SetParent(null);
        heldRb.bodyType = RigidbodyType2D.Dynamic;

        float t = chargeTimer / chargeTimeMax;
        float force = Mathf.Lerp(minForce, maxForce, t);

        float rad = throwAngle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        heldRb.AddForce(dir * force, ForceMode2D.Impulse);

        audioSource.Play();

        heldObject = null;
        heldRb = null;

        GameManager_2.instance.UseRight();

        //  关键补这一句
        StartCoroutine(SpawnDelay());
    }

    IEnumerator SpawnDelay()
    {
        canSpawn = false;
        yield return new WaitForSeconds(2f);
        canSpawn = true;
    }
}
