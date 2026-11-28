using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰霜打击
// Frost Strike
// 对一个随从造成$3点伤害。如果该随从死亡，发现一张冰霜符文牌。
// Deal $3 damage to a minion. If it dies, Discover a Frost Rune card.
internal static class Sim_RLK_025
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
