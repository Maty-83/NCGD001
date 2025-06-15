using Assets.Core.Enums;
using Assets.Helpers.Enums;
using Assets.Scripts.Objects.ScriptableObjects;
using Assets.Scripts.Objects.Spells;
using Assets.Scripts.Objects;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

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
        internal float curTimeBetweenAttacks = 0f;

        [Header("Audio Clips")]
        public AudioClip DeathAudioClip;

        internal bool isWatchingRight = false;
        public Dictionary<DamageType, IResistance> Resistancies { get; set; }
        public List<Weapon> OwnedWeapons { get; set; }
        public Dictionary<KeyCode, Weapon> BindedWeapons { get; set; }
        public List<Weapon> MeleeWeapons { get; internal set; }
        public List<Weapon> RangeWeapons { get; internal set; }
        public List<Weapon> ProtectiveWeapons { get; internal set; }
        public float MaxHP { get; private set; }
        public float MaxMana { get; private set; }
        public bool IsAlive { get; internal set; } = true;
        public bool IsMovingRight { get; internal set; } = true;
        public bool IsPaused { get; internal set; } = false ;

        internal SpriteRenderer renderer;
        internal Rigidbody2D rb;
        internal Animator animator;

        public virtual void RecieveDamage(IDamager weapon)
        {
            var amount = weapon.BaseDamage;
            if (Resistancies.ContainsKey(weapon.Type))
            {
                amount = amount * ( 1 / Resistancies[weapon.Type].GetResistance() );
            }

            HP -= amount;
        }

        public virtual void OnDeath(bool destroy = true)
        {
            if (IsAlive)
            {
                if (DeathAudioClip != null)
                {
                    AudioSource.clip = DeathAudioClip;
                    AudioSource.loop = false;
                    AudioSource.Play();
                }
                StartCoroutine(HandleDeath(destroy));
            }
        }

        private IEnumerator HandleDeath(bool destroy)
        {
            IsAlive = false;

            animator.SetTrigger("Die");
            yield return new WaitForSeconds(GetAnimationLength("Death"));

            if (destroy)
                Destroy(gameObject);
        }

        float GetAnimationLength(string animationName)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            int stateHash = stateInfo.shortNameHash;

            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (Animator.StringToHash(clip.name) == stateHash)
                {
                    return clip.length;
                }
            }

            return 0f;
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
            curTimeBetweenAttacks = Math.Max(minTimeBetweenBullets, minTimeBetweenMelee);

            rb = GetComponent<Rigidbody2D>();
            renderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            foreach (var weapon in DefaultWeapons)
            {
                BindedWeapons.Add(weapon.Binding, weapon);
                if (weapon.WeaponType == WeaponType.Melee)
                    MeleeWeapons.Add(weapon);
                else if (weapon.WeaponType == WeaponType.Range)
                    RangeWeapons.Add(weapon);
                else if (weapon.WeaponType == WeaponType.Protective)
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

            if(!IsPaused)
                HandleOrientation();
        }

        internal void FixedUpdate()
        {
            if (!IsAlive)
                return;

            if (!IsPaused)
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
                            if (IsMovingRight)
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
            }

            if (MaxMana > Mana)
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
            var controller = instance.GetComponent<IProjectile>();

            controller.Init(gameObject,
                weapon,
                instance.GetComponent<Rigidbody2D>(),
                new Vector2(dir.x, dir.y));

            animator.SetTrigger("Shoot");
            MakeWeaponSound(weapon);
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
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, weapon.Range);
            Vector2 forward = IsMovingRight ? Vector2.right : Vector2.left;

            animator.SetTrigger("Melee");

            foreach (var hit in hits)
            {
                Vector3 objectDirection = ( hit.transform.position - transform.position ).normalized;
                float angleTo = Vector3.Angle(forward, objectDirection);

                if (angleTo <= 90f)
                {
                    if (hit.gameObject == gameObject)
                        continue;

                    var entity = hit.gameObject.GetComponent<Entity>();
                    if (entity != null)
                        entity.RecieveDamage(weapon);
                }
            }

            MakeWeaponSound(weapon);
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
            MakeWeaponSound(weapon);
            curTimeBetweenAttacks = 0;
        }

        public virtual void MakeWeaponSound(Weapon weapon)
        {
            if (weapon.Sound == null)
                return;

            AudioSource.PlayOneShot(weapon.Sound);
        }


        private void HandleOrientation()
        {
            if (renderer == null)
                return;

            if (IsMovingRight)
                renderer.flipX = false;
            else
                renderer.flipX = true;
        }
    }
}