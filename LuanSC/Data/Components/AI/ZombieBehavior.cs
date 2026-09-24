using LuanSC.Objects;
using LuanSC.Objects.Weapons;
using Direction = LuanSC.Data.Direction;

namespace LuanSC.Data.Components.AI;

/// <summary>
/// Moves toward the nearest living player. Attacks when the weapon range reaches it.
/// </summary>
public class ZombieBehavior : IAIBehavior
{
    public int MaxSteps { get; set; } = 2;

    public IReadOnlyList<TurnAction> PlanTurn(GameObject self, ICombatContext context)
    {
        List<TurnAction> plan = new();

        GameObject target = FindNearestTarget(self, context);
        if (target is null) return plan;

        Weapon weapon = self.GetComponent<WeaponComponent>()?.Weapon;
        Point position = self.Position;

        for (int step = 0; step < MaxSteps; step++)
        {
            if (weapon is not null && TryGetAttackDirection(weapon, position, target.Position, out _))
                break; // already in range

            Direction? next = ChooseStep(position, target.Position, context);
            if (next is null) break; // blocked

            plan.Add(new TurnAction.Move(next.Value));
            position += next.Value.ToVector();
        }

        if (weapon is not null && TryGetAttackDirection(weapon, position, target.Position, out Direction facing))
        {
            plan.Add(new TurnAction.Face(facing));
            plan.Add(new TurnAction.Attack());
        }

        return plan;
    }

    private static GameObject FindNearestTarget(GameObject self, ICombatContext context)
    {
        GameObject best = null;
        int bestDistance = int.MaxValue;

        foreach (GameObject obj in context.Objects)
        {
            if (obj is not PlayerGameObject) continue;

            HPComponent hp = obj.GetComponent<HPComponent>();
            if (hp is null || !hp.IsAlive) continue;

            int distance = Distance(self.Position, obj.Position);
            if (distance < bestDistance)
            {
                best = obj;
                bestDistance = distance;
            }
        }

        return best;
    }

    private static Direction? ChooseStep(Point from, Point to, ICombatContext context)
    {
        Direction? best = null;
        int bestDistance = Distance(from, to);

        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Point next = from + direction.ToVector();
            int distance = Distance(next, to);

            if (distance < bestDistance && context.IsWalkable(next))
            {
                best = direction;
                bestDistance = distance;
            }
        }

        return best;
    }

    private static bool TryGetAttackDirection(Weapon weapon, Point from, Point target, out Direction facing)
    {
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            foreach (Point cell in weapon.Range.GetRotated(direction))
            {
                if (from + cell == target)
                {
                    facing = direction;
                    return true;
                }
            }
        }

        facing = Direction.UP;
        return false;
    }

    private static int Distance(Point a, Point b)
        => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
}