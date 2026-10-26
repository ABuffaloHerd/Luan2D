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
using LuanSC.Data.Components.AI;
using System.Diagnostics;
using System.Security.AccessControl;

namespace LuanSC.Scenes
{
    public partial class CombatScene : ICombatContext
    {
        public IReadOnlyList<GameObject> Objects => gameObjects;
        private List<GameObject> gameObjects = new();

        private EntityManager entityManager = new();

        /// <summary>
        /// Queue of game object sorted by speed. Pop this nigga to get the next object's turn
        /// </summary>
        private Queue<GameObject> queue = new();
        private GameObject currentControlledGameObject = null;
        private Queue<TurnAction> aiPlan;   // null means "no plan yet for this turn"
        private TimeSpan aiTimer = TimeSpan.Zero;

        /// <summary>
        /// In the future this will consume the combatsettings object to produce a gaming scene.
        /// </summary>
        private void InitObjects(CombatSettings settings)
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
                .Where(e => e.GetComponent<SpeedComponent>() is not null)
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

            // finish up the previous turn
            currentControlledGameObject?.GetComponent<EffectComponent>()?.TickEnd(msg => fightFeed.AddLine(msg));

            // Dequeue and run
            currentControlledGameObject = queue.Dequeue();
            currentControlledGameObject.Blink();

            aiPlan = null;
            aiTimer = TimeSpan.Zero;

            // Then update the turn order console.
            order.Clear();
            int y = 0;
            // print the current object's turn
            order.Print(0, y++, ColoredString.Parser.Parse($"[c:r f:Black][c:r b:Yellow]{currentControlledGameObject.Name}"));
            foreach (var obj in queue)
            {
                order.Print(0, y++, ColoredString.Parser.Parse(obj.Name));
            }

            // effect processing
            var effects = currentControlledGameObject.GetComponent<EffectComponent>();
            effects?.Tick();

            // reload action counters
            currentControlledGameObject.GetComponent<ActionComponent>()?.Refill();

            // report to the fight feed
            fightFeed.AddLine($"{currentControlledGameObject.Name}'s turn.");
        }

        /// <summary>
        /// remove the corpses of the dead instead of leaving them where they fall
        /// </summary>
        private void Cull()
        {
            foreach(GameObject obj in gameObjects)
            {
                if (obj.GetComponent<HPComponent>() is not null && !obj.GetComponent<HPComponent>().IsAlive)
                {
                    gameObjects.Remove(obj);
                    entityManager.Remove(obj);
                }
            }    
        }

        private static ActionType? ActionTypeOf(TurnAction action)
        {
            return action switch
            {
                TurnAction.Move => ActionType.MOVE,
                TurnAction.Attack => ActionType.ATTACK,
                _ => null // if they ain't declared it's free dawg it's lit
            };
        }

        private void Execute(GameObject actor, TurnAction action)
        {
            // Check if the actor has enough action points to perform the action
            ActionType? type = ActionTypeOf(action);
            ActionComponent actions = actor.GetComponent<ActionComponent>();

            if (type is not null && actions is not null && !actions.CanSpend(type.Value))
            {
                // Not enough action points, cannot perform the action
                GameEvents.Publish(new CombatEvent.GenericMessage($"{actor.Name} has no {type.Value} actions left."));
                return;
            }

            bool success = false; // track if successful and only spend points if it was

            // now execute the action
            switch (action)
            {
                case TurnAction.Move move:
                    Point target = actor.Position + move.Direction.ToVector();
                    if (IsWalkable(target))
                    {
                        actor.Position = target;
                        success = true;
                    }
                    break;

                case TurnAction.Face face:
                    actor.Direction = face.Direction;
                    break;

                case TurnAction.Attack:
                    Attack(actor);
                    success = true;
                    break;

                case TurnAction.EndTurn:
                    // Notify the fight feed that the turn has ended
                    fightFeed.AddLine($"{actor.Name} ends their turn.");
                    StartNextTurn();
                    break;
            }

            // now spend the points
            if (success && type is not null)
            {
                actions?.TrySpend(type.Value);
            }
        }

        private void UpdateAI(TimeSpan delta)
        {
#if DEBUG
            Debug.Print($"UpdateAI: {currentControlledGameObject.Name} ({currentControlledGameObject.Position})");
#endif
            AIComponent ai = currentControlledGameObject.GetComponent<AIComponent>();
            if (ai is null) return;

            aiTimer += delta;
            if (aiTimer < ai.ActionDelay) return;
            aiTimer = TimeSpan.Zero;

            // Plan late, after the effects tick. Poison can kill the actor first.
            if (aiPlan is null)
            {
                aiPlan = new Queue<TurnAction>(ai.Behavior.PlanTurn(currentControlledGameObject, this));
                aiPlan.Enqueue(new TurnAction.EndTurn()); // the plan always ends the turn
            }

            Execute(currentControlledGameObject, aiPlan.Dequeue());
        }

        public bool IsWalkable(Point targetPos)
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
            if (targetPos.X >= surface.Width || targetPos.Y >= surface.Height) return false;

            return true;
        }
    }
}
