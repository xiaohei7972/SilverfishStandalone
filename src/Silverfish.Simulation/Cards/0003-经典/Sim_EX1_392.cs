using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 战斗怒火
// Battle Rage
// 每有一个受伤的友方角色，便抽一张牌。
// Draw a card for each damaged friendly character.
internal static class Sim_EX1_392
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
