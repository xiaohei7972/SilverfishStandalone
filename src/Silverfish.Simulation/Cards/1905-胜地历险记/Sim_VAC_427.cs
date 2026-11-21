using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 甜筒殡淇淋
// Corpsicle
// 造成$3点伤害。消耗3份残骸，在你的回合结束时将本牌移回你的手牌。
// Deal $3 damage. Spend 3 Corpses to return this to your hand at the end of your turn.
internal static class Sim_VAC_427
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
