using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 后台保镖
// Backstage Bouncer
// 嘲讽。战吼：将一个友方随从变形成为本随从的复制。
// Taunt Battlecry: Transform a friendly minion into a copy of this.
internal static class Sim_JAM_014
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
