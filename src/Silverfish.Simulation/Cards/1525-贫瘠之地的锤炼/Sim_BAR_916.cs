using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 血岩碎片刺背野猪人
// Blood Shard Bristleback
// 吸血，战吼：如果你的牌库少于或等于10张，对一个随从造成6点伤害。
// Lifesteal. Battlecry: If your deck contains 10 or fewer cards, deal 6 damage to a minion.
internal static class Sim_BAR_916
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
