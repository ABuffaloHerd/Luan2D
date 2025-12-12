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
        private List<GameObject> controllableObjects = new();
        private List<GameObject> enemyObjects = new();

        private EntityManager entityManager = new();

        private PlayerGameObject testObject = null;

        private void Init()
        {
            surface.SadComponents.Add(entityManager);

            // Create test game object
            testObject = new(new ColoredGlyph(Color.Red, Color.Transparent, 'H'), 1);
            testObject.Weapon = WeaponRegistry.Katana();
            entityManager.Add(testObject);
        }
    }
}
