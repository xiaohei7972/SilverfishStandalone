using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 缩小射线工程师
// Shrinkmeister
// 战吼：在本回合中，使一个随从获得-3攻击力。
// Battlecry: Give a minion -3_Attack this turn.
internal static class Sim_GVG_011
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
