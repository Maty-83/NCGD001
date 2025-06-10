using UnityEngine;

namespace Assets.Scripts.Objects.Spells
{
    public class SpawnSpell : MonoBehaviour, IProjectile
    {
        [Header("Settings")]
        public AudioSource audioSource;
        public AudioClip spawnClip;

        public GameObject EnemyPrefab;
        public int EnemiesToSpawn = 5;
        public float SpawnRadius = 2f;
        public float SafeSpawnRadius = 0.5f;
        public float ProjectileSpeed = 10f;

        private GameObject Shooter;
        private IDamager Weapon;
        private Rigidbody2D rBody;
        private Vector2 Direction;
        public float Speed = 1000f;
        public float TTL { get; set; } = 1f;

        private bool isPaused = false;
        private bool isFlying = false;
        private bool hasHit = false;

        public void Init(GameObject shooter, IDamager weapon, Rigidbody2D rigid, Vector2 direction)
        {
            Shooter = shooter;
            Direction = direction;
            Weapon = weapon;
            rBody = rigid;
        }

        public virtual void AfterHit()
        {
            TTL = -1;
        }

        public virtual void Destroy()
        {
            Destroy(gameObject);
        }

        public virtual void Enter()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Exit()
        {
            throw new System.NotImplementedException();
        }

        public virtual void OnHit()
        {
            throw new System.NotImplementedException();
        }

        public void Pause()
        {
            isPaused = true;
        }

        public virtual void Play()
        {
            isPaused = false;
        }

        public virtual void Shoot()
        {
            if (!isPaused)
            {
                rBody.AddForce(( Direction.normalized * Speed ), ForceMode2D.Force);
                if (Mathf.Abs(rBody.linearVelocity.x) > Speed)
                {
                    rBody.linearVelocity = new Vector2((
                        Mathf.Sign(rBody.linearVelocity.x) * Speed ),
                        rBody.linearVelocity.y);
                }
            }
        }

        void Update()
        {
            if (TTL < 0 && !audioSource.isPlaying)
            {
                Destroy();
                return;
            }

            TTL -= Time.deltaTime;

            if (rBody != null && rBody.linearVelocity.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(rBody.linearVelocity.y, rBody.linearVelocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (hasHit) return;

            hasHit = true;

            SpawnEnemiesAround(transform.position);
            AfterHit();
        }

        private void SpawnEnemiesAround(Vector3 center)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(spawnClip);

            int spawned = 0;
            int maxAttempts = EnemiesToSpawn * 5;
            int attempts = 0;

            while (spawned < EnemiesToSpawn && attempts < maxAttempts)
            {
                attempts++;

                Vector2 randomPos = center + (Vector3) ( UnityEngine.Random.insideUnitCircle * SpawnRadius );

                Collider2D hit = Physics2D.OverlapCircle(randomPos, SafeSpawnRadius);

                if (hit == null)
                {
                    Instantiate(EnemyPrefab, randomPos, Quaternion.identity);
                    spawned++;
                }
            }
        }
    }
}
