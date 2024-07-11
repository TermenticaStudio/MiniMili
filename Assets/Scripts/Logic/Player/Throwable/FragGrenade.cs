using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Logic.Player.ThrowablesSystem
{
    public class FragGrenade : ThrowableObject
    {
        [SerializeField] private GameObject vfx;
        [SerializeField] private AudioClip explosionSFX;
        [SerializeField] private float damage = 20f;
        [SerializeField] private float range = 2;
        [SerializeField] private float fuzeTimer = 5f;

        [SerializeField] private UnityEvent OnExplode;

        public override void Throw(Player owner, Vector3 direction, float power)
        {
            base.Throw(owner, direction, power);
            StartFuse();
        }

        private void StartFuse()
        {
            StartCoroutine(ExplodeCoroutine());
        }

        private IEnumerator ExplodeCoroutine()
        {
            yield return new WaitForSeconds(fuzeTimer);

            AudioManager.Instance.Play2DSFX(explosionSFX, transform.position);
            Instantiate(vfx, transform.position, Quaternion.identity, null);

            var cols = Physics2D.OverlapCircleAll(transform.position, range);

            foreach (var col in cols)
            {
                // We need to check if frag is reachable to collider or not, so we use raycast to check that.
                var hit = Physics2D.Raycast(transform.position, col.transform.position);

                if (hit.collider == null)
                    continue;

                //ImpactCreator.CreateImpact(col, hit.point, hit.normal);

                var damagable = col.GetComponent<IDamagable>();
                var distance = Vector2.Distance(transform.position, hit.transform.position);
                damagable?.Damage(owner, Mathf.Lerp(damage, damage / 4f, distance / range), true);
            }

            OnExplode?.Invoke();
            Destroy(gameObject);
        }
    }
}