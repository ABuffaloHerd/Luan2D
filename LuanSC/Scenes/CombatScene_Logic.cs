using LuanSC.Data;
using LuanSC.Data.Components;
using LuanSC.Objects;
using LuanSC.Objects.Weapons;
using SadConsole.Entities;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class CombatScene
    {
        private List<GameObject> gameObjects = new();

        private EntityManager entityManager = new();

        /// <summary>
        /// Queue of game object sorted by speed. Pop this nigga to get the next object's turn
        /// </summary>
        private Queue<GameObject> queue = new();

        private GameObject currentControlledGameObject = null;

        /// <summary>
        /// In the future this will consume the combatsettings object to produce a gaming scene.
        /// </summary>
        private void Init(CombatSettings settings)
        {
            surface.SadComponents.Add(entityManager);

            foreach (var obj in settings.GameObjects)
            {
                gameObjects.Add(obj);
                entityManager.Add(obj);
            }

            // build the turn queue
            BuildTurnQueue();

            // Kick off the first turn or we're in trouble
            StartNextTurn();
        }

        private void BuildTurnQueue()
        {
            // objects in gameObjects sorted by speed
            List<GameObject> ordered = gameObjects
                .Where(e => e.GetComponent<SpeedComponent>() != null)
                .OrderByDescending(e => e.GetComponent<SpeedComponent>().Speed)
                .ToList();

            queue = new Queue<GameObject>(ordered);
        }

        private void StartNextTurn()
        {
            if (queue.Count <= 0)
            {
                BuildTurnQueue(); // the cycle begins anew
            }

            // Dequeue and run
            currentControlledGameObject = queue.Dequeue();
            currentControlledGameObject.Blink();

            // Then update the turn order console.
            order.Clear();
            int y = 0;
            // print the current object's turn
            order.Print(0, y++, ColoredString.Parser.Parse($"[c:r f:Black][c:r b:Yellow]{currentControlledGameObject.Name}"));
            foreach (var obj in queue)
            {
                order.Print(0, y++, ColoredString.Parser.Parse(obj.Name));
            }

            // report to the fight feed
            fightFeed.AddLine($"{currentControlledGameObject.Name}'s turn.");
        }

        private bool BoundsCheck(Point targetPos)
        {
            // checks if the target position is out of bounds or on top of another entity
            foreach (var obj in gameObjects)
            {
                // if it has collision
                if (obj.Position == targetPos)
                {
                    if (obj.GetComponent<CollisionComponent>() != null)
                    {
                        return false;
                    }
                }
            }

            if (targetPos.X < 0 || targetPos.Y < 0) return false;
            if (targetPos.X > surface.Width || targetPos.Y > surface.Height) return false;

            return true;
        }
    }
}
