using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 馆长
// The Curator
// 嘲讽。战吼：&抽取野兽牌，龙牌和鱼人牌各一张。
// Taunt Battlecry:& Draw a Beast, Dragon, and Murloc.
[CardSim("KAR_061")]
internal static class Sim_KAR_061
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrawMatchingEffect(new CardFilter { MinionType = MinionType.Beast }), new DrawMatchingEffect(new CardFilter { MinionType = MinionType.Dragon }), new DrawMatchingEffect(new CardFilter { MinionType = MinionType.Murloc })],
    };
}
