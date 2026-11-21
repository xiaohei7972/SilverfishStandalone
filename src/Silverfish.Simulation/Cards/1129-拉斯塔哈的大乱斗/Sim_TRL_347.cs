using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 诱饵射击
// Baited Arrow
// 造成$3点伤害。超杀：召唤一个5/5的魔暴龙。
// Deal $3 damage. Overkill: Summon a 5/5 Devilsaur.
internal static class Sim_TRL_347
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
