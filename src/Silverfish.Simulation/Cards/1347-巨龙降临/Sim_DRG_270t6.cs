using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 玛里苟斯的变形术
// Malygos's Polymorph
// 使一个随从变形成为1/1的绵羊。
// Transform a minion into a 1/1 Sheep.
internal static class Sim_DRG_270t6
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
