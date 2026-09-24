using LuanSC.Data;
using LuanSC.Data.Components;
using LuanSC.Data.Components.Attacks;
using LuanSC.Objects;
using LuanSC.Objects.Weapons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class CombatScene
    {
        private List<GameObject> FindTargets(GameObject user, Pattern range)
        {
            List<GameObject> targets = new();
            foreach (var cell in range.GetRotated(user.Direction))
            {
                int x = user.Position.X + cell.X;
                int y = user.Position.Y + cell.Y;

                // Now sample the game objects at that position
                foreach (var entity in entityManager.Entities)
                {
                    if (entity.Position.X == x && entity.Position.Y == y)
                    {
                        // Upcast to GameObject
                        GameObject gameobj = (GameObject)entity;
                        // Check that the entity is not the attacker itself
                        if (entity != user)
                        {
                            targets.Add(gameobj);
                        }
                    }
                }

#if DEBUG // DEBUG PRONT
                Debug.Print($"Targets for {user.Name}:");
                foreach (var target in targets)
                    Debug.Print($"  - {target.GetType().Name} at ({target.Position.X}, {target.Position.Y})");
#endif
            }

            return targets;
        }

        private void UseAbilities(GameObject user, IReadOnlyList<IAbility> abilities, IReadOnlyList<GameObject> targets)
        {
            foreach (var ability in abilities)
            {
                if (ability.CanUse(user))
                {
                    ability.Execute(user, targets);
                }
                else
                {
                    GameEvents.Publish(new CombatEvent.GenericMessage($"{user} cannot use {ability.Name} right now."));
                }
            }
        }

        public void Attack(GameObject attacker)
        {
            // bad block of null checks
            if (attacker is null) return;
            if (attacker.GetComponent<WeaponComponent>() is null) return; // you don't even have fists??

            // First use the attacker's weapon range to find targets
            Weapon weapon = attacker.GetComponent<WeaponComponent>().Weapon;
            if (weapon is null) return; // you have a weapon component but not a weapon?? how did you get this far tbh

            var targets = FindTargets(attacker, weapon.Range);
            
            // now we can execute order 66
            UseAbilities(attacker, weapon.Abilities, targets);
        }
    }
}
