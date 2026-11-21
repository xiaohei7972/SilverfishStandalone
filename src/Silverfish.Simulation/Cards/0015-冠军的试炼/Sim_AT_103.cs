using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 北海海怪
// North Sea Kraken
// 战吼：造成4点伤害。
// Battlecry: Deal 4 damage.
internal static class Sim_AT_103
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
