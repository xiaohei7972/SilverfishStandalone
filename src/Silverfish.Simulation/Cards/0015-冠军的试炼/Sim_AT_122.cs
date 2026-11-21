using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 穿刺者戈莫克
// Gormok the Impaler
// 战吼：如果你拥有至少四个其他随从，则造成4点伤害。
// Battlecry: If you have at least 4 other minions, deal 4 damage.
internal static class Sim_AT_122
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS, 4)],
        []);
    // </legacy-play-requirements>

}
