using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 叫嚣的中士
// Abusive Sergeant
// 战吼：在本回合中，使一个随从获得+2攻击力。
// Battlecry: Give a minion +2_Attack this turn.
internal static class Sim_CS2_188
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
