using UnityEngine;

namespace Logic.Player.ThrowablesSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ThrowableObject : MonoBehaviour
    {
        [SerializeField] private AudioClip throwSFX;

        private Rigidbody2D rigid;
        protected Player owner;

        public virtual void Throw(Player owner, Vector3 direction, float power)
        {
            this.owner = owner;
            rigid = GetComponent<Rigidbody2D>();
            AudioManager.Instance.Play2DSFX(throwSFX, transform.position);
            rigid.AddForce(direction * power);
        }
    }
}