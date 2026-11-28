using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大型法术蓝宝石
// Greater Sapphire Spellstone
// 选择一个友方随从，召唤三个它的复制。
// Summon 3 copies of a friendly minion.
internal static class Sim_LOOT_064t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
