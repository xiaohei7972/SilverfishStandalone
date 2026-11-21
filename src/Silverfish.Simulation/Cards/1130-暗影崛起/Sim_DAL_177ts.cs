using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 咒术师的召唤
// Conjurer's Calling
// 消灭一个随从。召唤两个法力值消耗相同的随从来 替换它。
// Destroy a minion. Summon 2 minions of the same Cost to replace it.
internal static class Sim_DAL_177ts
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
