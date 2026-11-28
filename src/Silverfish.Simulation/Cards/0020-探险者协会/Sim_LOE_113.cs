using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 鱼人恩典
// Everyfin is Awesome
// 使你的所有随从获得+2/+2。你每控制一个鱼人，本牌的法力值消耗便减少（1）点。
// Give your minions +2/+2. Costs (1) less for each Murloc you control.
[CardSim("LOE_113")]
internal static class Sim_LOE_113
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_LOE_113e.Rule.ApplyTo(StatTarget.FriendlyMinions),
        ],
        SelfCostReduction = SelfCostReduction.FriendlyMurlocs,
    };
}
