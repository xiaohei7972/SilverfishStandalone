using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雷矛特种兵
// Stormpike Commando
// 战吼：造成2点伤害。
// Battlecry: Deal 2 damage.
internal static class Sim_CS2_150
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
