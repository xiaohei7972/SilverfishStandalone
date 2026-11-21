using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 布置陷阱
// Spring the Trap
// 对一个随从造成$3点伤害，从你的牌库中施放一个奥秘。荣誉消灭：改为施放两个。
// Deal $3 damage to a minion and cast a Secret from your deck. Honorable Kill: Cast 2.
internal static class Sim_AV_224
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
