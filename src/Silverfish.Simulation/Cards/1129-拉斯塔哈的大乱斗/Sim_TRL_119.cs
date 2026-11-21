using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 野兽之心
// The Beast Within
// 使一个友方野兽获得+1/+1，使其随机攻击一个敌方随从。
// Give a friendly Beast +1/+1, then it attacks a random enemy minion.
internal static class Sim_TRL_119
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
