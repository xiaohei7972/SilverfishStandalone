using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 加基森摆渡人
// Gadgetzan Ferryman
// 连击：将一个友方随从移回你的手牌。
// Combo: Return a friendly minion to your hand.
internal static class Sim_CFM_693
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_FOR_COMBO), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
