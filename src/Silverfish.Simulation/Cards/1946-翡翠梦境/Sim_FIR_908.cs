using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火炭变色龙
// Charred Chameleon
// 战吼：如果你在本回合中使用过英雄技能，使一个友方随从获得+1/+2和突袭。
// Battlecry: If you've used your Hero Power this turn, give a friendly minion +1/+2 and Rush.
internal static class Sim_FIR_908
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
