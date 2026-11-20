namespace Silverfish.Simulation.Cards;

// 冰川裂片
// Glacial Shard
// 战吼： 冻结一个敌人。
// Battlecry: Freeze an_enemy.
[CardSim("UNG_205")]
internal static class Sim_UNG_205
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.EnemyCharacter,
        OnPlay = [new FreezeEffect(FreezeTarget.Selected)]
    };
}
