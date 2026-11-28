using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 北海海怪
// North Sea Kraken
// 战吼：造成4点伤害。
// Battlecry: Deal 4 damage.
[CardSim("AT_103")]
internal static class Sim_AT_103
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_NONSELF_TARGET)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.Selected, 4),
        ],
    };
}
