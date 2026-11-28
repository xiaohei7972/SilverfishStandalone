using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 圣殿执行者
// Temple Enforcer
// 战吼：使一个友方随从获得+3生命值。
// Battlecry: Give a friendly minion +3 Health.
internal static class Sim_VAN_EX1_623
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
