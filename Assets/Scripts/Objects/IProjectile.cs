using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Objects
{
    internal interface IProjectile
    {
        void Shoot();
        void OnHit();
        void AfterHit();
        void Destroy();
    }
}
