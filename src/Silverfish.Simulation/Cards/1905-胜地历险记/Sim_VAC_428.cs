using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 顺水漂流
// Go with the Flow
// 选择一个随从。如果是敌方随从，将其冻结；如果是友方随从，使其获得法术伤害+1。
// Choose a minion. If it's an enemy, Freeze it. If it's friendly, give it Spell Damage +1.
internal static class Sim_VAC_428
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
