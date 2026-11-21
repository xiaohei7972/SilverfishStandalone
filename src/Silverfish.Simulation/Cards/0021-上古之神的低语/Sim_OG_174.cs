using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 无面蹒跚者
// Faceless Shambler
// 嘲讽，战吼：复制一个友方随从的攻击力和生命值。
// Taunt Battlecry: Copy a friendly minion's Attack and Health.
internal static class Sim_OG_174
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
