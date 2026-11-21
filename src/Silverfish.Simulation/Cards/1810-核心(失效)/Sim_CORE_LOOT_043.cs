using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小型法术紫水晶
// Lesser Amethyst Spellstone
// 吸血 对一个随从造成$3点伤害。（受到来自你的卡牌的伤害后升级。）
// Lifesteal. Deal $3 damage to a minion. (Take damage from your cards to upgrade.)
internal static class Sim_CORE_LOOT_043
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
