using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 护园森灵
// Conservator Nymph
// 战吼：将一个友方树人变形成为5/5并具有嘲讽的古树。
// Battlecry: Transform a friendly Treant into a 5/5 Ancient with Taunt.
internal static class Sim_TTN_927
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_MUST_HAVE_TAG, (GAME_TAG)2831)],
        []);
    // </legacy-play-requirements>

}
