using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪恶魂笼
// Sinister Soulcage
// 使一个友方亡灵获得+2/+2。消耗5份残骸，召唤一个它的复制。
// Give a friendly Undead +2/+2. Spend 5 Corpses to summon a copy of it.
internal static class Sim_YOG_513
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.UNDEAD)],
        []);
    // </legacy-play-requirements>

}
