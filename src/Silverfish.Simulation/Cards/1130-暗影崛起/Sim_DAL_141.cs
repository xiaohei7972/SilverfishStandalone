using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 孤注一掷
// Desperate Measures
// 双生法术 随机施放一个圣骑士奥秘。
// Twinspell Cast a random Paladin Secret.
internal static class Sim_DAL_141
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_SECRET_ZONE_CAP_FOR_NON_SECRET)],
        []);
    // </legacy-play-requirements>

}
