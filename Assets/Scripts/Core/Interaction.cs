using System;
using UnityEngine;

namespace We80s.Core
{
    public interface IInteractive
    {
        BoxCollider BoxCollider { get; set; }
    }
    
    public interface IInteraction
    {
        void Execute();
    }

    public interface IBoundToPointInteraction : IInteraction
    {
        IInteractive Interactive { get; set; }
        event Action<IInteractive> OnInteract;
    }

    public interface IBoundToBoundInteraction : IInteraction
    {
        IInteractive A { get; set; }
        IInteractive B { get; set; }
        event Action<IInteractive, IInteractive> OnInteract;
    }
}