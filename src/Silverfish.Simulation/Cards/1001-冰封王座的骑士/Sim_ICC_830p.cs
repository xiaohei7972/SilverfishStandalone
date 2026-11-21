using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 虚空形态
// Voidform
// 造成$2点伤害。在你使用一张牌后，复原此技能。
// Deal $2 damage. After you play a card, refresh this.
internal static class Sim_ICC_830p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
