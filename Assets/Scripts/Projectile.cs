using Mirror.Examples;
using Mirror;
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

    public void Init(float speed, float range, float damage)
    {
        rigid = GetComponent<Rigidbody2D>();
        initPos = transform.position;

        rigid.velocity = transform.right * speed;
        this.range = range;
        this.damage = damage;
    }

    public void RegisterOwner(PlayerInfo owner)
    {
        this.owner = owner;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var playerCol = collision.collider.GetComponentInParent<PlayerInfo>();

        if (playerCol != null)
        {
            if (playerCol == owner)
                return;

            playerCol.GetComponent<PlayerHealth>().CmdDamage(damage);
        }

        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        // return to prefab pool
        NetworkServer.UnSpawn(gameObject);
        PrefabPool.singleton.Return(gameObject);
    }
}