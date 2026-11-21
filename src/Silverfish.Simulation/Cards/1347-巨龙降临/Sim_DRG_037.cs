using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 菲里克·飞刺
// Flik Skyshiv
// 战吼：消灭一个随从及其所有的复制（无论它们在哪）。
// Battlecry: Destroy a minion and all copies of it (wherever they are).
internal static class Sim_DRG_037
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
