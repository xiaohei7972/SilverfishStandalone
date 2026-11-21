using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 棱彩透镜
// Prismatic Lens
// 从你的牌库中抽一张随从牌和一张法术牌，交换其法力值消耗。
// Draw a minion and a spell from your deck. Swap their Costs.
internal static class Sim_BOT_436
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
