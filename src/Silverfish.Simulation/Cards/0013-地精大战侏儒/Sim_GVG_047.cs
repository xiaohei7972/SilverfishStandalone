using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗中破坏
// Sabotage
// 随机消灭一个敌方随从，连击：并且摧毁你对手的武器。
// Destroy a random enemy minion. Combo: And your opponent's weapon.
internal static class Sim_GVG_047
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
