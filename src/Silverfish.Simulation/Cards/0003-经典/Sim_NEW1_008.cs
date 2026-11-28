using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 知识古树
// Ancient of Lore
// 抉择：抽两张牌；或者恢复#7点生命值。
// Choose One - Draw 2 cards; or Restore #7 Health.
[CardSim("NEW1_008")]
internal static class Sim_NEW1_008
{
    internal static PlayReqSet PlayReqs { get; } = new([], []);

    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("NEW1_008a", [new DrawEffect(2)]),
            new("NEW1_008b", [new HealEffect(HealTarget.Selected, 7)])
            {
                PlayReqs = [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)]
            }
        ]
    };
}
