using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗金教鸦人祭司
// Kabal Talonpriest
// 战吼：使一个友方随从获得+3生命值。
// Battlecry: Give a friendly minion +3 Health.
internal static class Sim_CFM_626
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
