using UnityEngine;

namespace Logic.Player.WeaponsSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : PoolObject
    {
        private float range;
        private float damage;

        private Rigidbody2D rigid;
        private Vector2 initPos;
        private Player owner;
        private ProjectileTrail trail;

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
            trail = GetComponentInChildren<ProjectileTrail>();

            transform.SetPositionAndRotation(pos, rot);
            initPos = transform.position;
            gameObject.SetActive(true);

            rigid.velocity = transform.right * speed;
            this.range = range;
            this.damage = damage;

            trail?.StartTimer();

            isInit = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isInit)
                return;

            ImpactCreator.CreateImpact(collision.collider, collision.GetContact(0).point, collision.GetContact(0).point - (Vector2)transform.position);
            DestroySelf();

            var damagable = collision.gameObject.GetComponent<IDamagable>();

            if (damagable != null)
                damagable.Damage(owner, damage);
        }

        private void DestroySelf()
        {
            gameObject.SetActive(false);
        }
    }
}