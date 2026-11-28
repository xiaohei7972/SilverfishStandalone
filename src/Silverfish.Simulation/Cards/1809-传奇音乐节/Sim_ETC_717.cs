using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 悦耳嘻哈
// Harmonic Hip Hop
// 造成$1点伤害。使你的武器获得+3攻击力。（每回合切换。）
// Deal $1 damage. Give your weapon +3 Attack. (Swaps each turn.)
internal static class Sim_ETC_717
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_NONSELF_TARGET)],
        []);
    // </legacy-play-requirements>

}
