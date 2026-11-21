using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 法术翡翠
// Emerald Spellstone
// 召唤三只3/3的狼。（使用一个奥秘后升级。）
// Summon three 3/3_Wolves. (Play a Secret to upgrade.)
internal static class Sim_LOOT_080t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
