using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components;

public interface IHealingListener
{
    void OnHealingReceived(HealingRecord healing);
}
