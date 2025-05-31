
using Assets.Scripts.Entities;
using UnityEngine;

namespace Assets.Scripts.Objects.Spells
{
    public class HealSpell : MonoBehaviour, IProtectiveSpell
    {
        private Entity Caster = null;
        private bool casted = false;
        private int TTL = 100;

        private void Update()
        {
            if (casted)
            {
                if (Caster != null)
                {
                    transform.position = Caster.transform.position;
                }

                if (TTL <= 0)
                {
                    AfterCast();
                }
                TTL--;
            }
        }

        public void AfterCast()
        {
            Destroy(gameObject);
        }

        public void Cast(Entity caster, ScriptableObjects.Weapon spell)
        {
            if (caster.HP + 20 > caster.MaxHP)
                caster.HP = caster.MaxHP;
            else
                caster.HP += 20;

            casted = true;
            Caster = caster;
        }
    }
}
