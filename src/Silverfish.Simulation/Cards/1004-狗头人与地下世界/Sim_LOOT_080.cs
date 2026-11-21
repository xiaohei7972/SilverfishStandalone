using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小型法术翡翠
// Lesser Emerald Spellstone
// 召唤两只3/3的狼。（使用一个奥秘后升级。）
// Summon two 3/3_Wolves. (Play a Secret to upgrade.)
internal static class Sim_LOOT_080
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
