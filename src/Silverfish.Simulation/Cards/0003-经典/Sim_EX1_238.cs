using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 闪电箭
// Lightning Bolt
// 造成$3点伤害，过载：（1）
// Deal $3 damage. Overload: (1)
[CardSim("EX1_238")]
internal static class Sim_EX1_238
{
    // 闪电箭可以选择友方角色。
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_TARGET_TO_PLAY)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.Selected, 3)]
    };
}
