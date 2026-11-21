using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 吸血鬼之吻
// Vampyr's Kiss
// 使一个随从获得+3攻击力。本技能消耗残骸，而非法 力值。
// Give a minion +3 Attack. This costs Corpses instead of Mana.
internal static class Sim_JAIL_446hp
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
