using Assets.Core.Enums;
using Assets.Helpers.Enums;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Assets.Scripts.Objects.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponScriptableObject", order = 1)]
    public class Weapon : ScriptableObject, IDamager
    {
        [SerializeField] public  GameObject Prefab;
        [SerializeField] public  Sprite PreviewImage;
        [SerializeField] public  float Range; //Non-positive means unlimited
        [SerializeField] public  KeyCode Binding;
        [SerializeField] public  WeaponType WeaponType;
        [SerializeField] public  float ManaCost;
        [SerializeField] public  string Decription;
        [SerializeField] public  float BuyCost;

        [SerializeField] private DamageType DamageType;
        [SerializeField] private float Damage;

        public DamageType Type { get { return DamageType; } set { DamageType = value; } } 
        public float BaseDamage { get { return Damage; } set { Damage = value; } }
        public bool DestroysProjectiles { get { return WeaponType.Melee == WeaponType; } set { } }
        public int ProjectileTeam { get { return 0; } set { } }
    }
}
