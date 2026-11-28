using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 妙手空空
// Pilfered Power
// 每控制一个友方随从，便获得一个空的法力水晶。
// Gain an empty Mana Crystal for each friendly minion.
internal static class Sim_CFM_616
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_HAND_NOT_FULL), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
