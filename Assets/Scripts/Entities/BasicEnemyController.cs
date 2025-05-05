using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    class BasicEnemyController : Entity
    {
        [SerializeField] int HealthBarTimer = 10;
        [SerializeField] BarController barControll = null;

        private int timer = 0;

        private new void Start()
        {
            base.Start();
            barControll.SetValues(HP, false, 0, HP);
        }
        private new void Update()
        {
            base.Update();

            if (barControll != null)
            {
                if (timer > 1)
                {
                    timer--;
                }
                else if (timer == 1)
                {
                    barControll.SetValues(HP, false);
                    timer = 0;
                }
            }
        }

        public override void RecieveDamage(IDamager weapon)
        {
            base.RecieveDamage(weapon);
            if (HP <= 0)
                return;

            barControll.SetValues(HP, true);
            timer = HealthBarTimer;
        }
    }
}
