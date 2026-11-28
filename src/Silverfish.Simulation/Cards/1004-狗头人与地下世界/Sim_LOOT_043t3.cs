using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大型法术紫水晶
// Greater Amethyst Spellstone
// 吸血 对一个随从造成$7点伤害。
// Lifesteal. Deal $7 damage to a minion.
internal static class Sim_LOOT_043t3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
