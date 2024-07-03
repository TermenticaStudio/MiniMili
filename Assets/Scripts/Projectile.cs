using Logic.Player;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : PoolObject
{
    private const string BLOOD_IMPACT = "Blood_Impact";
    private const string NORMAL_IMPACT = "Normal_Impact";

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
            DestroySelf();
    }

    public override void OnDisable()
    {
        if (isInit)
            isInit = false;

        base.OnDisable();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInit)
            return;

        CreateImpact(collision.collider, collision.GetContact(0).point, collision.GetContact(0).normal);

        DestroySelf();
    }

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

        GetTriggerContactPoint(collision, out var pos, out var normal);
        CreateImpact(collision, pos, normal);
    }

    private void CreateImpact(Collider2D collider, Vector2 position, Vector2 normal)
    {
        var id = string.Empty;

        if (collider.gameObject.TryGetComponent<HitPoint>(out var com))
        {
            id = BLOOD_IMPACT;
        }
        else
        {
            id = NORMAL_IMPACT;
        }

        var instance = PrefabPool.Instance.Get(id);
        instance.transform.SetPositionAndRotation(position, Quaternion.FromToRotation(Vector2.up, normal));
    }

    private void DestroySelf()
    {
        gameObject.SetActive(false);
    }

    public void GetTriggerContactPoint(Collider2D collider, out Vector2 point, out Vector2 normal)
    {
        point = collider.ClosestPoint(transform.position);
        normal = (Vector2)transform.position - point;
    }
}