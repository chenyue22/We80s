using System;
using UnityEngine;

namespace We80s.GameActor
{
    unsafe public class Player : Actor
    {
        public PlayerAttributes* playerAttributes;
        public int playerAge;

        private PlayerController playerController;

        protected override ActorController CreateController()
        {
            playerController = new PlayerController();
            return playerController;
        }

        private void Update()
        {
            playerController.Update();
        }
    }   
}
