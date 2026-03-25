using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

public interface IEffect
{
    string Name { get; set; }
    int Duration { get; set; } // duration in turns. -1 = permanant, 0 = expired

    bool Expired => Duration == 0;

    // if implemented, this will be called when the effect is applied to the owner. This allows the effect to hurt the owner, heal the owner, or do anything else it wants when applied.
    void OnApply(GameObject owner);

    // if implemented, this will be called when the effect is removed from the owner. This allows the effect to hurt the owner really bad once more or heal the owner really good once more.
    void OnRemove(GameObject owner);

    // if implemented, this will be called at the start of the owner's turn. To regenerate, hurt or whatever.
    void OnTurnStart(GameObject owner);

    // what if the effect was delayed so you had to think in game? Hopefully this adds """depth""" to the game.
    void OnTurnEnd(GameObject owner);

    // if implemented, this will be called before damage is applied to the owner. This allows the effect to modify or negate the damage.
    void OnIncomingDamage(GameObject owner, DamageRecord damage);
}
