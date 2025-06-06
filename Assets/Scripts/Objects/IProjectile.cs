using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    internal interface IProjectile
    {
        void Init(GameObject shooter, IDamager weapon, Rigidbody2D rigid, Vector2 direction);
        void Shoot();
        void OnHit();
        void AfterHit();
        void Destroy();
    }
}
