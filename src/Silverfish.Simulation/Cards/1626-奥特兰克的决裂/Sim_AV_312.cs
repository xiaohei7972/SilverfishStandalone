using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 献祭召唤者
// Sacrificial Summoner
// 战吼：消灭一个友方随从。从你的牌库中召唤一个法力值消耗增加（1）点的随从。
// Battlecry: Destroy a friendly minion. Summon a minion from your deck that costs (1) more.
internal static class Sim_AV_312
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
