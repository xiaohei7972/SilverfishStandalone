using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 奥秘吞噬者
// Eater of Secrets
// 战吼：摧毁所有敌方奥秘。每摧毁一个，便获得+1/+1。
// Battlecry: Destroy all enemy Secrets. Gain +1/+1 for each.
[CardSim("OG_254")]
internal static class Sim_OG_254
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DestroySecretsAndBuffEffect(Sim_OG_254e.CardId),
        ],
    };
}
