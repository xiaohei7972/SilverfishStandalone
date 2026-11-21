using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 纳多克斯长老
// Elder Nadox
// 战吼：消灭一个友方亡灵，你的所有随从获得其攻击力。
// Battlecry: Destroy a friendly Undead. Your minions gain its Attack.
internal static class Sim_RLK_658
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.UNDEAD)],
        []);
    // </legacy-play-requirements>

}
