using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private float range;
    private float damage;

    private Rigidbody2D rigid;
    private Vector2 initPos;
    private PlayerInfo owner;

    private void Update()
    {
        if (Vector2.Distance(initPos, transform.position) > range)
        {
            DestroySelf();
        }
    }

    public void Init(PlayerInfo owner, float speed, float range, float damage)
    {
        this.owner = owner;

        rigid = GetComponent<Rigidbody2D>();
        initPos = transform.position;

        rigid.velocity = transform.right * speed;
        this.range = range;
        this.damage = damage;
    }

    //[ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DestroySelf();
    }

    //[ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        Destroy(gameObject);
        //PrefabPool.Instance.Return("Bullet", gameObject);
    }
}