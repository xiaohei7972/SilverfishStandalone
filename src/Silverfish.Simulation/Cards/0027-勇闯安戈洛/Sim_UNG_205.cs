using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰川裂片
// Glacial Shard
// 战吼： 冻结一个敌人。
// Battlecry: Freeze an_enemy.
[CardSim("UNG_205")]
internal static class Sim_UNG_205
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new FreezeEffect(FreezeTarget.Selected)]
    };
}
