using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 拉特维亚护甲师
// Latorvian Armorer
// 战吼：对一个敌方随从造成2点伤害。如果该随从死亡，&获得5点护甲值。
// Battlecry: Deal 2 damage to an enemy minion. If it dies,& gain 5 Armor.
internal static class Sim_TLC_606
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
