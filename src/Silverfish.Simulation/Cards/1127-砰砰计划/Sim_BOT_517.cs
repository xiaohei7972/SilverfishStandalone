using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 引力翻转
// Topsy Turvy
// 使一个随从的攻击力和生命值 互换。
// Swap a minion's Attack and Health.
internal static class Sim_BOT_517
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
