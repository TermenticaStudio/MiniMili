using Mirror;
using Mono.CSharp;
using UnityEngine;

namespace Logic.Player.WeaponsSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : PoolObject
    {
        private Collider myCollider;
        private float range;
        private float damage;

        private Rigidbody2D rigid;
        private Vector2 initPos;
        private Player owner;
        private ProjectileTrail trail;

        private bool isInit;
        private void OnEnable()
        {
            if(myCollider == null)
            {
                myCollider = GetComponent<Collider>();
            }
        }
        private void Start()
        {
            if (isInit)
            {
                foreach (Collider co in owner.GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(myCollider, co);
            }


        }
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

        public void Init(Player owner, Vector3 pos, Quaternion rot, float speed, float range, float damage, float trailLength)
        {
            this.owner = owner;
            rigid = GetComponent<Rigidbody2D>();
            trail = GetComponentInChildren<ProjectileTrail>();
            foreach (Collider co in owner.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(myCollider, co);

            transform.SetPositionAndRotation(pos, rot);
            initPos = transform.position;
            gameObject.SetActive(true);

            rigid.velocity = transform.right * speed;
            this.range = range;
            this.damage = damage;

            trail?.StartTimer(speed, trailLength);


            isInit = true;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isInit)
                return;
            if (FindObjectOfType<SceneObjectsContainer>().isServer)
            {
                //ImpactCreator.CreateImpact(collision.collider, collision.GetContact(0).point, collision.GetContact(0).point - (Vector2)transform.position);

                var damagable = collision.gameObject.GetComponent<IDamagable>();

                if (damagable != null)
                    damagable.Damage(owner, damage);
                    
                       
            }
            ImpactCreator.CreateImpact(collision.collider, collision.GetContact(0).point, collision.GetContact(0).point - (Vector2)transform.position);
            DestroySelf();

            
        }
        [Server]
        private void DestroySelf()
        {
     /*       if (FindObjectOfType<SceneObjectsContainer>().isServer)
            {
                NetworkServer.UnSpawn(gameObject);
            }*/
            NetworkServer.UnSpawn(gameObject);
            Release();
        }
    }
}