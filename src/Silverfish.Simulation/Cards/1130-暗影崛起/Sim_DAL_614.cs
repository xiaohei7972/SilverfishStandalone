using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 狗头人跟班
// Kobold Lackey
// 战吼：造成2点伤害。
// Battlecry: Deal 2 damage.
[CardSim("DAL_614")]
internal static class Sim_DAL_614
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_TARGET_IF_AVAILABLE)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.Selected, 2)]
    };
}
