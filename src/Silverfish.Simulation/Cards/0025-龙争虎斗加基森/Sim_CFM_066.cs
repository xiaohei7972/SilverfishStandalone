namespace Silverfish.Simulation.Cards;

// 暗金教侍从
// Kabal Lackey
// 战吼： 在本回合中，你使用的下一个奥秘的法力值消耗为（1）点。
// Battlecry: The next Secret you play this turn costs (1).
[CardSim("CFM_066")]
internal static class Sim_CFM_066
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new AddTemporaryCostAuraEffect(EntityKind.Spell, CostModifierMode.Set, 1,
                SecretOnly: true, ConsumedOnPlay: true),
        ],
    };
}
