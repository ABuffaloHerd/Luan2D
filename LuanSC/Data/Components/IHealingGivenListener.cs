using System;
using System.Collections.Generic;
using System.Text;
using LuanSC.Data.Combat;

namespace LuanSC.Data.Components;

public interface IHealingGivenListener
{
    void OnHealingGiven(HealingRecord healing);
}
