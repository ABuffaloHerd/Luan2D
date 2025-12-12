using LuanSC.Objects;
using LuanSC.Objects.Weapons;
using LuanSC.Scenes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    public interface IAttackComponent : IComponent
    {
        CombatRecord Execute(
            GameObject owner,
            Weapon weapon,
            IReadOnlyList<GameObject> targets,
            CombatScene scene);
    }
}
