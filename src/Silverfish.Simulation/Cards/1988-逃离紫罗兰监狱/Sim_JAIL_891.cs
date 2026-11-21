using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 虚空冲击
// Void Blast
// 对一个随从造成$3点伤害。如果该随从死亡，获取一张虚空灵魂。
// Deal $3 damage to a minion. If it dies, get a Void Soul.
internal static class Sim_JAIL_891
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
