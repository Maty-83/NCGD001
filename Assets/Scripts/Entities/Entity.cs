using Assets.Core.Enums;
using System.Collections.Generic;
using UnityEngine;
using Assets.Helpers.Enums;
using Assets.Scripts.Objects.ScriptableObjects;
using Assets.Scripts.Objects.Spells;

namespace Assets.Scripts.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] public float HP = 100; 
        [SerializeField] public float Mana = 100;
        [SerializeField] public float ManaRecoverySpeed = 0.05f;

        [Header("Status field")]
        [SerializeField] private List<Weapon> DefaultWeapons;

        [Header("Timers")]
        public float minTimeBetweenBullets = 0.2f;
        internal float curTimeBetweenBullets = 0;
        public float minTimeBetweenMelee = 0.4f;
        internal float curTimeBetweenMelee = 0;


        internal bool isWatchingRight = false;
        public Dictionary<DamageType, int> Resistancies {  get; set; }
        public Dictionary<KeyCode, Weapon> Weapons {  get; set; }
        public List<Weapon> MeleeWeapons { get; private set; }
        public List<Weapon> RangeWeapons { get; private set; }
        public List<Weapon> ProtectiveWeapons { get; private set; }
        public float MaxHP { get; private set; }
        public float MaxMana { get; private set; }

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
            //Some animation here

            Destroy(gameObject);
        }

        internal void Start()
        {
            MaxHP = HP;
            MaxMana = Mana;
            Resistancies = new();
            Weapons = new();
            MeleeWeapons = new();
            RangeWeapons = new();
            ProtectiveWeapons = new();

            foreach (var weapon in DefaultWeapons)
            {
                Weapons.Add(weapon.Binding, weapon);
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
            if (HP <= 0)
            {
                OnDeath();
            }
        }

        internal void FixedUpdate()
        {
            foreach (var weapon in Weapons)
            {
                if (!Input.GetKey(weapon.Key))
                    continue;

                if (weapon.Value.WeaponType == WeaponType.Melee)
                {
                    if (curTimeBetweenMelee > minTimeBetweenMelee)
                    {
                        Vector2 dir = Vector2.left;
                        if (isWatchingRight)
                            dir = Vector2.right;

                        OnMelee(weapon.Value, dir);
                    }
                }
                else if (weapon.Value.WeaponType == WeaponType.Range)
                {
                    if (curTimeBetweenBullets > minTimeBetweenBullets && weapon.Value.ManaCost < Mana)
                    {
                        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        var dir = -1 * ( gameObject.transform.position - mouse );
                        OnShoot(weapon.Value, dir);
                        Mana -= weapon.Value.ManaCost;
                    }
                }
                else if (weapon.Value.WeaponType == WeaponType.Protective)
                {
                    if (curTimeBetweenBullets > minTimeBetweenBullets && weapon.Value.ManaCost < Mana)
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

            curTimeBetweenBullets += Time.fixedDeltaTime;
            curTimeBetweenMelee += Time.fixedDeltaTime;
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

            controller.Shoot();
            curTimeBetweenBullets = 0;
        }

        public virtual void OnMelee(Weapon weapon, Vector2 dir)
        {

            var hits = Physics2D.RaycastAll(transform.position, Vector2.right, weapon.Range);
            Debug.DrawLine(transform.position, dir * 10f, Color.red);

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
            curTimeBetweenMelee = 0;
        }

        public virtual void OnProtectiveSpell(Weapon weapon)
        {
            var instance = Instantiate(weapon.Prefab);
            instance.transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                instance.transform.position.z);

            instance.GetComponent<IProtectiveSpell>().Cast(this, weapon);
            curTimeBetweenBullets = 0;
        }
    }
}
