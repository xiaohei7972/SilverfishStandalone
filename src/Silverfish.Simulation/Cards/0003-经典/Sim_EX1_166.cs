using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 丛林守护者
// Keeper of the Grove
// 抉择：造成2点伤害；或者沉默一个随从。
// Choose One - Deal_2_damage; or Silence a minion.
[CardSim("EX1_166")]
internal static class Sim_EX1_166
{
    internal static PlayReqSet PlayReqs { get; } = new([], []);

    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("EX1_166a", [new DamageEffect(DamageTarget.Selected, 2)])
            {
                PlayReqs = [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)]
            },
            new("EX1_166b", [new SilenceEffect()])
            {
                PlayReqs = [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)]
            }
        ]
    };
}
