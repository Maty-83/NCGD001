using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.Entities.Behaviour
{
    internal interface IBehaviour
    {
        void Push(Vector2 direction);
    }
}
