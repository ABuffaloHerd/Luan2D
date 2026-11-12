using System.Reflection;

namespace LuanSC.Data.Levels;

/// <summary>
/// The sections of the battle selection screen, shown in this order. Empty ones are left out.
/// </summary>
public enum BattleCategory
{
    TUTORIAL, // how play boring game
    SPECIAL,  // angry birds golden eggs level type shit
    STORY,   // the main campaign. i'm sure i'll get to it some day.
    PARADOX, // character highlight
    TEST, // TESTIFICATE
}

/// <summary>
/// Lists a LevelRegistry factory on the battle selection screen. The method must be public, static, take no arguments and return CombatSettings.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class BattleAttribute(string title, BattleCategory category) : Attribute
{
    public string Title { get; } = title;
    public BattleCategory Category { get; } = category;

    /// <summary>
    /// Position within the category, lowest first. Reflection doesn't promise any method order, so ties go by title.
    /// </summary>
    public int Order { get; init; }
}

public sealed record BattleEntry(string Title, BattleCategory Category, Func<CombatSettings> Create)
{
    public override string ToString() => Title; // what a ListBox draws for this item
}

/// <summary>
/// Every level tagged with [Battle], sorted by category and then order.
/// </summary>
public static class BattleCatalog
{
    public static readonly IReadOnlyList<BattleEntry> All = Build();

    private static List<BattleEntry> Build()
    {
        List<(MethodInfo Method, BattleAttribute Tag)> tagged = new();

        foreach (MethodInfo method in typeof(LevelRegistry).GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (method.GetCustomAttribute<BattleAttribute>() is not BattleAttribute tag) continue;

#if !DEBUG
            // test levels are for debug builds only
            if (tag.Category == BattleCategory.TEST) continue;
#endif

            tagged.Add((method, tag));
        }

        return tagged
            .OrderBy(x => x.Tag.Category)
            .ThenBy(x => x.Tag.Order)
            .ThenBy(x => x.Tag.Title)
            .Select(x => new BattleEntry(x.Tag.Title, x.Tag.Category, x.Method.CreateDelegate<Func<CombatSettings>>()))
            .ToList();
    }
}
