using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 末日回旋镖
// Doomerang
// 对一个随从投掷你的武器。武器会造成伤害，然后移回你的手牌。
// Throw your weapon at a minion. It deals its damage, then returns to_your hand.
internal static class Sim_ICC_233
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_WEAPON_EQUIPPED), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
