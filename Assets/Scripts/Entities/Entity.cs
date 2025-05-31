using Assets.Core.Enums;
using System.Collections.Generic;
using UnityEngine;
using Assets.Helpers.Enums;
using Assets.Scripts.Objects.ScriptableObjects;
using Assets.Scripts.Objects.Spells;
using System.Collections;

namespace Assets.Scripts.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [Header("Common entity settings")]
        [SerializeField] public AudioSource AudioSource;
        [SerializeField] public float HP = 100; 
        [SerializeField] public float Mana = 100;
        [SerializeField] public float ManaRecoverySpeed = 0.05f;
        [SerializeField] public float ShootKnockbackForce = 10f;

        [Header("Status field")]
        [SerializeField] private List<Weapon> DefaultWeapons;

        [Header("Timers")]
        //internal float curTimeBetweenBullets = 0;
        public float minTimeBetweenBullets = 0.2f;
        public float minTimeBetweenMelee = 0.4f;
        internal float curTimeBetweenAttacks = 0;


        internal bool isWatchingRight = false;
        public Dictionary<DamageType, int> Resistancies {  get; set; }
        public List<Weapon> OwnedWeapons { get; set; }
        public Dictionary<KeyCode, Weapon> BindedWeapons {  get; set; }
        public List<Weapon> MeleeWeapons { get; private set; }
        public List<Weapon> RangeWeapons { get; private set; }
        public List<Weapon> ProtectiveWeapons { get; private set; }
        public float MaxHP { get; private set; }
        public float MaxMana { get; private set; }
        public bool IsAlive { get; internal set; } =  true;
        public bool IsMovingRight { get; internal set; } = true;

        internal SpriteRenderer renderer;
        internal Rigidbody2D rb;
        internal Animator animator;

        private Vector3 previousPos = Vector3.zero;

        public virtual void RecieveDamage(IDamager weapon)
        {
            var amount = weapon.BaseDamage;
            if (Resistancies.ContainsKey(weapon.Type))
            {
                amount = amount * (1/Resistancies[weapon.Type] );
            }

            HP -= amount;
        }

        public virtual void OnDeath()
        {
            Destroy(gameObject);
            //DestroyWithAnimation();
        }

        public void DestroyWithAnimation()
        {
            animator.SetBool("IsAlive", false);
            StartCoroutine(WaitForAnimationAndDestroy());
        }

        IEnumerator WaitForAnimationAndDestroy()
        {
            // Wait for the current state's duration
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length); // Not always safe if the animation hasn't changed yet

            Destroy(gameObject);
        }

        internal void Start()
        {
            MaxHP = HP;
            MaxMana = Mana;
            Resistancies = new();
            OwnedWeapons = new();
            BindedWeapons = new();
            MeleeWeapons = new();
            RangeWeapons = new();
            ProtectiveWeapons = new();

            rb = GetComponent<Rigidbody2D>();
            renderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            previousPos = transform.position;

            foreach (var weapon in DefaultWeapons)
            {
                BindedWeapons.Add(weapon.Binding, weapon);
                if(weapon.WeaponType == WeaponType.Melee)
                    MeleeWeapons.Add(weapon);
                else if(weapon.WeaponType == WeaponType.Range)
                    RangeWeapons.Add(weapon);
                else if(weapon.WeaponType ==  WeaponType.Protective)
                    ProtectiveWeapons.Add(weapon);
            }
        }

        internal void Update()
        {
            if (!IsAlive)
                return;

            if (HP <= 0)
            {
                OnDeath();
                IsAlive = false;
            }

            HandleOrientation();
        }

        internal void FixedUpdate()
        {
            foreach (var weapon in BindedWeapons)
            {
                if (!Input.GetKey(weapon.Key))
                    continue;

                if (weapon.Value.WeaponType == WeaponType.Melee)
                {
                    if (curTimeBetweenAttacks > minTimeBetweenMelee)
                    {
                        Vector2 dir = Vector2.left;
                        if (isWatchingRight)
                            dir = Vector2.right;

                        OnMelee(weapon.Value, dir);
                    }
                }
                else if (weapon.Value.WeaponType == WeaponType.Range)
                {
                    if (curTimeBetweenAttacks > minTimeBetweenBullets && weapon.Value.ManaCost < Mana)
                    {
                        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        var dir = -1 * ( gameObject.transform.position - mouse );
                        OnShoot(weapon.Value, dir);
                        Mana -= weapon.Value.ManaCost;
                    }
                }
                else if (weapon.Value.WeaponType == WeaponType.Protective)
                {
                    if (curTimeBetweenAttacks > minTimeBetweenBullets && weapon.Value.ManaCost < Mana)
                    {
                        OnProtectiveSpell(weapon.Value);
                        Mana -= weapon.Value.ManaCost;
                    }
                }
            }

            if(MaxMana > Mana)
            {
                Mana += ManaRecoverySpeed;
            }

            curTimeBetweenAttacks += Time.fixedDeltaTime;
        }

        public virtual void OnShoot(Weapon weapon, Vector2 dir)
        {
            if (weapon.Prefab == null)
                return;

            var instance = Instantiate(weapon.Prefab);
            instance.transform.position = gameObject.transform.position;
            var controller = instance.GetComponent<ProjectileController>();        

            controller.Init(gameObject,
                weapon,
                instance.GetComponent<Rigidbody2D>(),
                new Vector2(dir.x, dir.y));

            animator.SetTrigger("Shoot");
            MakeWeaponSound(instance, weapon);
            controller.Shoot();
            ShootKnockBack(dir * -1, ShootKnockbackForce);
            curTimeBetweenAttacks = 0;
        }

        public virtual void ShootKnockBack(Vector2 dir, float force)
        {
            rb.AddForce(dir.normalized * force, ForceMode2D.Force);
        }

        public virtual void OnMelee(Weapon weapon, Vector2 dir)
        {
            var hits = Physics2D.RaycastAll(transform.position, IsMovingRight ? Vector2.right : Vector2.left, weapon.Range);
            Debug.DrawLine(transform.position, dir * 10f, Color.red);
            animator.SetTrigger("Melee");

            if (hits.Length != 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject == gameObject)
                        continue;

                    var entity = hit.rigidbody.gameObject.GetComponent<Entity>();
                    entity.RecieveDamage(weapon);
                }
            }
            curTimeBetweenAttacks = 0;
        }

        public virtual void OnProtectiveSpell(Weapon weapon)
        {
            var instance = Instantiate(weapon.Prefab);
            instance.transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                instance.transform.position.z);

            instance.GetComponent<IProtectiveSpell>().Cast(this, weapon);
            MakeWeaponSound(instance, weapon);
            curTimeBetweenAttacks = 0;
        }

        public virtual void MakeWeaponSound(GameObject instance, Weapon weapon)
        {
            if (weapon.Sound == null)
                return;

            AudioSource.PlayOneShot(weapon.Sound);
        }


        private void HandleOrientation()
        {
            if (renderer == null)
                return;

            if(rb.linearVelocityX == 0)
            {
                if((previousPos - transform.position).x < 0)
                    renderer.flipX = false;
                else
                    renderer.flipX = true;
            }
            else if (rb.linearVelocityX > 0)
                renderer.flipX = false;
            else if (rb.linearVelocityX < 0)
                renderer.flipX = true;

            previousPos = transform.position;
        }
    }
}
