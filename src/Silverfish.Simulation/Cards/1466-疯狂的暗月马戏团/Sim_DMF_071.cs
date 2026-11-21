using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// “赤烟”腾武
// Tenwu of the Red Smoke
// 战吼：将一个友方随从移回你的手牌，在本回合中，其法力值消耗为（1）点。
// Battlecry: Return a friendly minion to your hand. It costs (1) this turn.
internal static class Sim_DMF_071
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
