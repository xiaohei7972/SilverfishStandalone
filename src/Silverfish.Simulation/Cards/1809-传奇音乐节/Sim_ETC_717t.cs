using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 刺耳嘻哈
// Dissonant Hip Hop
// 造成$3点伤害。使你的武器获得+1攻击力。（每回合切换。）
// Deal $3 damage. Give your weapon +1 Attack. (Swaps each turn.)
internal static class Sim_ETC_717t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_NONSELF_TARGET)],
        []);
    // </legacy-play-requirements>

}
