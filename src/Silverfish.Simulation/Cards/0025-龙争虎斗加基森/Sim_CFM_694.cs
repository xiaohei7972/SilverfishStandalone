using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗影大师
// Shadow Sensei
// 战吼：使一个潜行的随从获得+2/+2。
// Battlecry: Give a Stealthed minion +2/+2.
internal static class Sim_CFM_694
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_STEALTHED_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
