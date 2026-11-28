using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 无拘之魂
// Unshackle Soul
// 消灭一个随从。如果你在本牌在你手牌中时使用过对手卡牌的复制，本牌的法力值消耗为（1）点。
// Destroy a minion. If you played a copy of an opponent's card while holding this, this costs (1).
internal static class Sim_JAIL_433
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
