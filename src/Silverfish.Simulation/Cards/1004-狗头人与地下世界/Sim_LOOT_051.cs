using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小型法术玉石
// Lesser Jasper Spellstone
// 对一个随从造成$2点伤害。@（获得3点护甲值后升级。）
// Deal $2 damage to a minion. @(Gain 3 Armor to upgrade.)
internal static class Sim_LOOT_051
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
