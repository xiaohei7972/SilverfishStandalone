using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 绝望哀歌
// Dirge of Despair
// 对一个角色造成$3点伤害。如果该角色死亡，从你的牌库中召唤一个恶魔。
// Deal $3 damage to a character. If it dies, summon a Demon from your deck.
internal static class Sim_ETC_082
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
