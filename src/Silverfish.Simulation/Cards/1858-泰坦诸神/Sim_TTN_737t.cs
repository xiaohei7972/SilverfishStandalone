using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 鲜血符文
// Runes of Blood
// 消灭一个敌方随从。本随从和你的英雄获得其生命值。
// Destroy an enemy minion. This minion and your hero gain its Health.
internal static class Sim_TTN_737t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
