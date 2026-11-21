using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 劫持
// Nab
// 对一个随从造成 $3点伤害。如果该随从死亡，将一张该随从的法力值消耗为（2）的复制洗入你的牌库。
// Deal $3 damage to a minion. If it dies, shuffle a copy of it into your deck that costs (2).
internal static class Sim_JAIL_225
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
