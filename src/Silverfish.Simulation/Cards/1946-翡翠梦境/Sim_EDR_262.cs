using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灵魂联结
// Spirit Bond
// 对一个随从造成$3点伤害。如果该随从死亡，召唤一只3/2并具有突袭的狼。
// Deal $3 damage _to a minion. If it dies, summon a 3/2 Wolf with Rush.
internal static class Sim_EDR_262
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
