using UnityEngine;

namespace Assets.Scripts.Entities.Behaviour
{
    internal interface IBehaviour
    {
        void Push(Vector2 direction);
        void Pause(bool isPaused);
    }
}
