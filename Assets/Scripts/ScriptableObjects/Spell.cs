using Assets.Core.Enums;
using UnityEngine;

namespace Assets.Scripts.Objects.Weapon
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpellScriptableObject", order = 1)]
    public class Spell : ScriptableObject
    {
        [SerializeField] private Sprite PreviewImage;
        [SerializeField] private Animation FireAnimation;
        [SerializeField] private DamageType DamageType;
        [SerializeField] private float Damage;
        [SerializeField] private MonoBehaviour ControllScript;
    }
}
