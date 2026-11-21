using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 投掷利刃
// Throw Glaive
// 对一个随从造成$2点伤害。如果该随从死亡，则将本牌的一张临时复制置入你的手牌。
// Deal $2 damage to a minion. If it dies, add a_Temporary copy of this to your hand.
internal static class Sim_DMF_225
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
