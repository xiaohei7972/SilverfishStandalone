using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 殉船
// Down with the Ship
// 造成$3点伤害。随机将两张疫病牌洗入你对手的牌库。
// Deal $3 damage. Shuffle two random Plagues into your opponent's deck.
internal static class Sim_TTN_454
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
