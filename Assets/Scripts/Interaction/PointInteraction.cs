using System;
using UnityEngine;
using We80s.Core;

namespace We80s.Interaction
{
    public struct PointInteraction : IBoundToPointInteraction
    {
        public IInteractive Interactive { get; set; }
        public event Action<IInteractive> OnInteract;

        private Vector3 position;
        private float sqrStopDistance;

        public PointInteraction(IInteractive interactive, Vector3 p, float stopDistance = 0.1f, Action<IInteractive> e = null)
        {
            Interactive = interactive;
            position = p;
            sqrStopDistance = stopDistance * stopDistance;
            OnInteract = e;
        }

        public void Execute()
        {
            if ((Interactive.BoxCollider.center - position).sqrMagnitude < sqrStopDistance)
            {
                var f = Interactive.BoxCollider.transform.forward;
                var d = (position - Interactive.BoxCollider.transform.position).normalized;
                f.x = d.x;
                f.z = d.z;
                Interactive.BoxCollider.transform.forward = f;
                OnInteract?.Invoke(Interactive);
            }
        }
    }   
}