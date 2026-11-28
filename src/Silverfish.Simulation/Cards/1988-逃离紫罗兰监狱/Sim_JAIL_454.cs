using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 紧急手术
// Emergency Surgery
// 选择一个敌方随从。召唤四个3/1并具有吸血的亡灵攻击该随从。
// Choose an enemy minion. Summon four 3/1 Undead with Lifesteal that attack it.
internal static class Sim_JAIL_454
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
