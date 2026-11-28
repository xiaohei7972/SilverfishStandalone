using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 泥潭守护者
// Mire Keeper
// 抉择：召唤一个2/2的泥浆怪；或者获得一个空的法力水晶。
// Choose One - Summon a 2/2 Slime; or Gain an empty Mana Crystal.
[CardSim("OG_202")]
internal static class Sim_OG_202
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("OG_202a", [new SummonEffect("OG_202c")]) { PlayReqs = [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)] },
            new("OG_202b", [new ChangeManaEffect(ManaOperation.GainEmpty, 1)]),
        ],
    };
}
