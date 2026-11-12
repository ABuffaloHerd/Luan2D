using LuanSC.Data.Components.Effects;
using LuanSC.Objects;
using LuanSC.Objects.Items;

namespace LuanSC.Data.Components;

/// <summary>
/// The items the owner carries, stacked by name. No component, no items and no picking any up.
/// Held effects are kept in step with the stacks: one effect per item, so two charms give two effects.
/// </summary>
public class InventoryComponent : IComponent, IOnTurnStartListener
{
    private sealed class Stack
    {
        public Item Item;
        public int Count;
        public readonly List<IEffect> Held = new(); // the effects this stack has put on the owner
    }

    private readonly List<Stack> stacks = new();

    public string Name => "Inventory Component";
    public GameObject Owner { get; private set; }

    /// <summary>
    /// Every stack, oldest first.
    /// </summary>
    public IEnumerable<(Item Item, int Count)> Items => stacks.Select(s => (s.Item, s.Count));

    public int Count(Item item) => Find(item)?.Count ?? 0;

    public void Add(Item item, int count = 1)
    {
        if (count <= 0) return;

        Stack stack = Find(item);
        if (stack is null)
        {
            stack = new Stack { Item = item };
            stacks.Add(stack);
        }

        stack.Count += count;
        RefreshHeld();
    }

    /// <returns>false if there aren't that many. Nothing is taken in this case.</returns>
    public bool Remove(Item item, int count = 1)
    {
        Stack stack = Find(item);
        if (stack is null || stack.Count < count) return false;

        stack.Count -= count;
        RefreshHeld(); // before the stack goes, so its effects come off

        if (stack.Count == 0) stacks.Remove(stack);
        return true;
    }

    /// <summary>
    /// Makes the held effects on the owner match the stacks again. Safe to call any time.
    /// Add and Remove call it, and so does anything that wipes effects behind our back (FormComponent.Revert).
    /// </summary>
    public void RefreshHeld()
    {
        EffectComponent effects = Owner?.GetComponent<EffectComponent>();
        if (effects is null) return;

        foreach (Stack stack in stacks)
        {
            if (stack.Item.Held is null) continue;

            // something else removed them, e.g. a transform clearing every effect
            stack.Held.RemoveAll(effect => !effects.Effects.Contains(effect));

            while (stack.Held.Count < stack.Count)
            {
                IEffect effect = stack.Item.Held();
                effects.Apply(effect);
                stack.Held.Add(effect);
            }

            while (stack.Held.Count > stack.Count)
            {
                IEffect effect = stack.Held[^1];
                stack.Held.RemoveAt(stack.Held.Count - 1);
                effects.Remove(effect);
            }
        }
    }

    // catches anything that stripped a held effect during the round, e.g. a cleanse
    public void OnTurnStart() => RefreshHeld();

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
        RefreshHeld();
    }

    private Stack Find(Item item) => stacks.FirstOrDefault(s => s.Item.Name == item.Name);
}
