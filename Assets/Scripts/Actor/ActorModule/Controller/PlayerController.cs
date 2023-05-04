using UnityEngine;
using UnityEngine.AI;
using We80s.Core;
using We80s.GameEvent;
using We80s.Interaction;
using We80s.Utils;

namespace We80s.GameActor
{
    public class PlayerController : ActorController
    {
        private IInteraction interaction;

        private void InteractWithActor(IInteractive a, IInteractive b)
        {
            interaction = null;
            agent.isStopped = true;
            agent.updateRotation = true;
        }

        private void InteractWithEventBound(IInteractive a, IInteractive b)
        {
            interaction = null;
            agent.isStopped = true;
            agent.updateRotation = true;
        }

        public void Update()
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                var ray = GameMain.mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, float.MaxValue, LayerUtils.actor))
                {
                    Actor dest;
                    if (hit.collider.TryGetComponent(out dest))
                    {
                        agent.destination = dest.transform.position;
                        agent.updateRotation = false;
                        agent.isStopped = false;
                        interaction = new ActorInteraction(interactive, dest.Controller.interactive, InteractWithActor);
                    }
                }

                if (Physics.Raycast(ray, out hit, float.MaxValue, LayerUtils.eventBound))
                {
                    EventBound dest;
                    if (hit.collider.TryGetComponent(out dest))
                    {
                        agent.destination = dest.transform.position;
                        agent.updateRotation = false;
                        agent.isStopped = false;
                        interaction = new EventBoundInteraction(interactive, dest, InteractWithEventBound);
                    }
                }
                
                if (Physics.Raycast(ray, out hit, float.MaxValue, LayerUtils.ground))
                {
                    agent.destination = hit.point;
                    agent.updateRotation = false;
                    agent.isStopped = false;
                }
            }

            if (interaction != null)
            {
                interaction.Execute();
            }
        }
    }
}