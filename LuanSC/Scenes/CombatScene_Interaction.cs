using LuanSC.Data.Components;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class CombatScene
    {
        public void Attack(GameObject attacker)
        {
            // First use the attacker's weapon range to find targets
            // copied code from the overlay render method

            // List of targets
            List<GameObject> targets = new();

            // Check that it is controllable object's turn
            if (currentControlledGameObject.GetComponent<ControllableComponent>() is null) return;

            // check for a weapon component
            if (currentControlledGameObject is null) return;
            WeaponComponent w = currentControlledGameObject.GetComponent<WeaponComponent>();
            if (w == null) return;

            var pattern = w.Weapon.Range;
            foreach (var cell in pattern.GetRotated(currentControlledGameObject.Direction))
            {
                int x = currentControlledGameObject.Position.X + cell.X;
                int y = currentControlledGameObject.Position.Y + cell.Y;

                // Now sample the game objects at that position
                foreach (var entity in entityManager.Entities)
                {
                    if (entity.Position.X == x && entity.Position.Y == y)
                    {
                        // Upcast to GameObject
                        GameObject gameobj = (GameObject)entity;
                        // Check that the entity is not the attacker itself
                        if (entity != attacker)
                        {
                            targets.Add(gameobj);
                        }
                    }
                }
            }

            // Debug print the targets
#if DEBUG
            Debug.Print("Attack targets:");
            foreach (var target in targets)
            {
                Debug.Print($"- {target.GetType().Name} at ({target.Position.X}, {target.Position.Y})");
            }
#endif
            // Big bad block of null checks
            WeaponComponent weapon = attacker.GetComponent<WeaponComponent>();
            if (weapon is null) return;

            // now do the damaging via the weapon component
            foreach(var component in weapon.Weapon.Components)
            {
                component.Execute(attacker, weapon.Weapon, targets, this);
            }
        }
    }
}
