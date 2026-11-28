using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小型法术蓝宝石
// Lesser Sapphire Spellstone
// 选择一个友方随从，召唤一个它的复制。@（过载三个法力水晶后升级。）
// Summon 1 copy of a friendly minion. @(Overload 3 Mana Crystals to upgrade.)
internal static class Sim_LOOT_064
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
