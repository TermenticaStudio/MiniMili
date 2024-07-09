using System;
using UnityEngine;

namespace Logic.Player.BodyDismantle
{
    public class BodyDismantle : MonoBehaviour
    {
        [SerializeField] private Dismantle[] dismantles;
        [SerializeField] private GameObject bloodParticle;
        [SerializeField] private float dismantleForce;

        private Player player;

        [Serializable]
        public class Dismantle
        {
            public GameObject Part;
            public Vector2 ForceDirection;
        }

        private void Start()
        {
            player = GetComponent<Player>();

            player.Health.OnDie += DismantleBody;
            player.Health.OnRevive += FixDismantles;
        }

        private void OnDestroy()
        {
            player.Health.OnDie -= DismantleBody;
            player.Health.OnRevive -= FixDismantles;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                FixDismantles();
        }

        private void DismantleBody()
        {
            foreach (var item in dismantles)
            {
                var copy = Instantiate(item.Part, item.Part.transform.position, item.Part.transform.rotation, null);
                var rigid = copy.GetComponent<Rigidbody2D>() ?? copy.AddComponent<Rigidbody2D>();
                rigid.velocity = player.Rigidbody.velocity;
                rigid.isKinematic = false;
                rigid.AddForce(item.ForceDirection * dismantleForce, ForceMode2D.Impulse);
                var collider = copy.GetComponentInChildren<Collider2D>();
                collider.isTrigger = false;
                Destroy(copy, 5f);

                foreach (var des in copy.GetComponentsInChildren<DestroyOnDismantle>())
                    Destroy(des.gameObject);

                item.Part.SetActive(false);

                var bloodPoses = copy.GetComponentsInChildren<BloodPosition>();

                foreach (var pos in bloodPoses)
                    Instantiate(bloodParticle, pos.transform.position, pos.transform.rotation, pos.transform);
            }
        }

        private void FixDismantles()
        {
            foreach (var item in dismantles)
                item.Part.SetActive(true);
        }
    }
}