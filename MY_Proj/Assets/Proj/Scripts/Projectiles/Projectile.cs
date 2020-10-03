using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.CharacterControls.AttackBehaviours;
using Proj.CharacterControls.States;

namespace Proj.Projectiles {
    public class Projectile : MonoBehaviour {
        public float speed;
        public GameObject muzzlePrefabs;
        public GameObject hitPrefab;
        public AudioClip shotSFX; // sound
        public AudioClip hitSFX;

        private bool collided;
        private Rigidbody projectileRigidbody;
        private float elapsedTime;
        private float lifeTime = 10.0f;

        public AttackBehaviour attackBehaviour;
        public GameObject owner;
        public GameObject target;

        // Start is called before the first frame update
        void Start() {
            elapsedTime = 0.0f;
            projectileRigidbody = GetComponent<Rigidbody>();

            if(owner != null) {
                Collider projectileCollider = GetComponent<Collider>();
                Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>();

                foreach(Collider collider in ownerColliders)
                    Physics.IgnoreCollision(projectileCollider, collider);
            }

            if(muzzlePrefabs != null) {
                GameObject muzzleVFX = Instantiate(muzzlePrefabs, transform.position, Quaternion.identity);
                muzzleVFX.transform.forward = gameObject.transform.forward;

                ParticleSystem particleSystem = muzzleVFX.GetComponent<ParticleSystem>();
                if(particleSystem)
                    Destroy(muzzleVFX, particleSystem.main.duration);
                else
                    Destroy(muzzleVFX, 0.2f);
            }

            if(shotSFX != null && GetComponent<AudioSource>())
                GetComponent<AudioSource>().PlayOneShot(shotSFX);

            if(target != null)
                transform.LookAt(target.transform.position);
        }

        // Makes this projectile to move.
        private void FixedUpdate() {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > lifeTime)
                Destroy(gameObject);

            if(target != null)
                transform.LookAt(target.transform.position);

            if(speed != 0 && projectileRigidbody != null)
                projectileRigidbody.position += (transform.forward) * (speed *Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision) {
            if(collided) return;

            collided = true;

            Collider projectileCollider = GetComponent<Collider>();
            projectileCollider.enabled = false;

            if(hitSFX != null && GetComponent<AudioSource>())
                GetComponent<AudioSource>().PlayOneShot(hitSFX);

            speed = 0;
            // rigidbody에 의해 위치가 제어되지 않고 OnCollisionEnter가 더 이상 호출되지 않음.
            projectileRigidbody.isKinematic = true;

            ContactPoint contact = collision.contacts[0];
            Quaternion contactRotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 contactPosition = contact.point;

            if(hitPrefab != null) {
                GameObject hitVFX = Instantiate(hitPrefab, contactPosition, contactRotation);
                ParticleSystem particleSystem = hitVFX.GetComponent<ParticleSystem>();

                if(particleSystem != null)
                    Destroy(hitVFX, particleSystem.main.duration);
                else {
                    ParticleSystem childParticleSystem = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(hitVFX, childParticleSystem.main.duration);
                }
            }

            IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
            if(damagable != null)
                damagable.TakeDamage(attackBehaviour?.damage ?? 0, null);

            StartCoroutine(DestroyParticle(0.0f));
        }

        public IEnumerator DestroyParticle(float waitTime) {
            if(transform.childCount > 0 && waitTime != 0) {
                List<Transform> childs = new List<Transform>();

                foreach(Transform t in transform.GetChild(0).transform)
                    childs.Add(t);

                while(transform.GetChild(0).localScale.x > 0) {
                    yield return new WaitForSeconds(0.01f);
                    transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                    for(int i = 0; i < childs.Count; i++)
                        childs[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }

            yield return new WaitForSeconds(waitTime);
            Destroy(gameObject);
        }
    }
}
