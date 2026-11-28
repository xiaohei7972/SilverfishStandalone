using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灰舌杀手
// Ashtongue Slayer
// 战吼：在本回合中，使一个潜行的随从获得+3攻击力和免疫。
// Battlecry: Give a Stealthed minion +3 Attack and Immune this turn.
internal static class Sim_BT_702
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_STEALTHED_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
