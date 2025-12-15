using LuanSC.Data.Components;
using LuanSC.Objects;
using LuanSC.Objects.Weapons;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
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

        private PlayerGameObject currentControlledGameObject = null;

        /// <summary>
        /// In the future this will consume the combatsettings object to produce a gaming scene.
        /// </summary>
        private void Init()
        {
            surface.SadComponents.Add(entityManager);

            // Create test game object
            currentControlledGameObject = new(new ColoredGlyph(Color.Red, Color.Transparent, 'H'), 1);
            currentControlledGameObject.Weapon = WeaponRegistry.Katana();
            currentControlledGameObject.GetComponent<HPComponent>().Max = 500;
            currentControlledGameObject.GetComponent<HPComponent>().Current = 10;
            gameObjects.Add(currentControlledGameObject);
            entityManager.Add(currentControlledGameObject);
        }
    }
}
