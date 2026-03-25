using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

public class BarrierEffect : IEffect
{
    public int Duration { get; set; }

    public string Name => "Barrier";

    public GameObject Owner { get; private set; }

    public void OnApply()
    {
        throw new NotImplementedException();
    }

    public void OnRemove()
    {
        throw new NotImplementedException();
    }

    public void OnTurnEnd(Action<string> report = null)
    {
        throw new NotImplementedException();
    }

    public void OnTurnStart(Action<string> report = null)
    {
        throw new NotImplementedException();
    }

    public void SetOwner(GameObject owner)
    {
        throw new NotImplementedException();
    }
}
