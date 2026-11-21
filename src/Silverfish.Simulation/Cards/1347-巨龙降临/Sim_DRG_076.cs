using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 无面腐蚀者
// Faceless Corruptor
// 突袭。战吼：将你的一个随从变形成为本随从的复制。
// Rush. Battlecry: Transform one of your minions into a copy of this.
internal static class Sim_DRG_076
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
