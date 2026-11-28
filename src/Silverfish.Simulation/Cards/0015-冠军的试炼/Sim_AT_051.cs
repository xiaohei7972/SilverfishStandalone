namespace Silverfish.Simulation.Cards;

// 元素毁灭
// Elemental Destruction
// 对所有随从造成$4到$5点伤害。过载：（2）
// Deal $4-$5 damage to all minions. Overload: (2)
[CardSim("AT_051")]
internal static class Sim_AT_051
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new RandomDamageRangeEffect(RandomCharacterPool.AllMinions, null, 4, 5) { HitEachCandidate = true },
        ],
    };
}
