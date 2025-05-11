using Assets.Scripts.Entities;

namespace Assets.Scripts.Objects.Spells
{
    public interface IProtectiveSpell
    {
        public void Cast(Entity caster, Assets.Scripts.Objects.ScriptableObjects.Weapon spell);
        public void AfterCast();
    }
}
