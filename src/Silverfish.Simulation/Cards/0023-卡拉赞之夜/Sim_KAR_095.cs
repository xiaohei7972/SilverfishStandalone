using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 机械动物管理员
// Zoobot
// 战吼：随机使一个友方野兽，龙和鱼人获得+1/+1。
// Battlecry: Give a random friendly Beast, Dragon, and Murloc +1/+1.
[CardSim("KAR_095")]
internal static class Sim_KAR_095
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EnchantMinionsByTypeEffect([MinionType.Beast, MinionType.Dragon, MinionType.Murloc], Sim_KAR_095e.Rule.ApplyTo(StatTarget.Selected))],
    };
}
