using System;
using We80s.Core;

namespace We80s.Interaction
{
    public struct EventBoundInteraction : IInteraction
    {
        public IInteractive A { get; set; }
        public IInteractive B { get; set; }
        public event Action<IInteractive, IInteractive> OnInteract;
        
        public EventBoundInteraction(IInteractive a, IInteractive b, Action<IInteractive, IInteractive> e)
        {
            A = a;
            B = b;
            OnInteract = e;
        }

        public void Execute()
        {
            var f = A.BoxCollider.transform.forward;
            var d = (B.BoxCollider.transform.position - A.BoxCollider.transform.position).normalized;
            f.x = d.x;
            f.z = d.z;
            A.BoxCollider.transform.forward = f;
            
            if (A.BoxCollider.bounds.Intersects(B.BoxCollider.bounds))
            {
                OnInteract?.Invoke(A, B);
            }
        }
    }   
}