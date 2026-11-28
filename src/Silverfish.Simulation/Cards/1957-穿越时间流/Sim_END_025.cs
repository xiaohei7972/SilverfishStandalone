using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 永时火焰箭
// Eternal Firebolt
// 吸血。对一个随从造成$3点伤害。如果该随从死亡，在你的回合结束时将本牌移回你的手牌。
// Lifesteal Deal $3 damage to a minion. If it dies, return this to your hand at the end of your turn.
internal static class Sim_END_025
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
