using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灵媒术
// Seance
// 选择一个随从，将一张它的复制置入你的手牌。
// Choose a minion. Add_a copy of it to your hand.
internal static class Sim_TRL_097
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
