using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    public interface IComponent
    {
        string Name { get; }
        GameObject Owner { get; }
        void SetOwner(GameObject owner);
    }
}
