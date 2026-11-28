using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火箭靴
// Rocket Boots
// 使一个随从获得突袭。抽 一张牌。
// Give a minion Rush. Draw a card.
internal static class Sim_BOT_067
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
