using Logic.Player;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private float range;
    private float damage;

    private Rigidbody2D rigid;
    private Vector2 initPos;
    private Player owner;

    private bool isInit;

    private void Update()
    {
        if (!isInit)
            return;

        if (Vector2.Distance(initPos, transform.position) > range)
        {
            DestroySelf();
        }
    }

    private void OnDisable()
    {
        if (isInit)
            isInit = false;
    }

    public void Init(Player owner, Vector3 pos, Quaternion rot, float speed, float range, float damage)
    {
        this.owner = owner;

        rigid = GetComponent<Rigidbody2D>();

        transform.SetPositionAndRotation(pos, rot);
        initPos = transform.position;
        gameObject.SetActive(true);

        rigid.velocity = transform.right * speed;
        this.range = range;
        this.damage = damage;

        isInit = true;
    }

    //[ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInit)
            return;

        DestroySelf();
    }

    //[ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInit)
            return;

        var damagable = collision.GetComponent<IDamagable>();

        if (damagable != null)
        {
            if (damagable.Damage(owner, damage))
                DestroySelf();
        }
    }

    //[Server]
    private void DestroySelf()
    {
        //NetworkServer.UnSpawn(gameObject);
        //Destroy(gameObject);
        PrefabPool.Instance.Return("Bullet", gameObject);
    }
}