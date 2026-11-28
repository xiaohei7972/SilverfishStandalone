using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 树皮术
// Barkskin
// 使一个随从 获得+3生命值。 获得3点护甲值。
// Give a minion +3 Health. Gain 3 Armor.
internal static class Sim_LOOT_047
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
