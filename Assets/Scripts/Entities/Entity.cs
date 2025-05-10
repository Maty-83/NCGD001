using Assets.Core.Enums;
using Assets.Scripts.Objects.Weapon;
using Assets.Scripts.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] public float HP = 100; 
        [SerializeField] public float Mana = 100;
        public Dictionary<DamageType, int> Resistancies {  get; set; }
        public Dictionary<string, IDamager> Weapons {  get; set; }

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
            Resistancies = new();
            Weapons = new();
        }

        internal void Update()
        {
            if (HP <= 0)
            {
                OnDeath();
            }
        }
    }
}
