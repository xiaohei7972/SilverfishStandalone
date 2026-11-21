using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 毒花少年！
// Poison Bloom!
// 使一个友方随从获得+2生命值和吸血。
// Give a friendly minion +2 Health and Lifesteal.
internal static class Sim_JAM_027b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
