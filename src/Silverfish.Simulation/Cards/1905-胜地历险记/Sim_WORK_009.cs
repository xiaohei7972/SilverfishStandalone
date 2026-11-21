using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 月度魔范员工
// Imployee of the Month
// 战吼：使一个友方随从获得吸血。
// Battlecry: Give a friendly minion Lifesteal.
internal static class Sim_WORK_009
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
