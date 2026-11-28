using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 展览馆法师
// Menagerie Magician
// 战吼：随机使一个友方野兽，龙和鱼人获得+2/+2。
// Battlecry: Give a random friendly Beast, Dragon, and Murloc +2/+2.
[CardSim("KAR_702")]
internal static class Sim_KAR_702
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EnchantMinionsByTypeEffect([MinionType.Beast, MinionType.Dragon, MinionType.Murloc], Sim_KAR_702e.Rule.ApplyTo(StatTarget.Selected))],
    };
}
