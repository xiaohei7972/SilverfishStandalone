using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 麦迪文的男仆
// Medivh's Valet
// 战吼： 如果你控制一个奥秘，则造成3点伤害。
// Battlecry: If you control a Secret, deal 3 damage.
[CardSim("KAR_092")]
internal static class Sim_KAR_092
{
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS, 1), new(ErrorType.REQ_NONSELF_TARGET)],
        []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.Selected, 3)],
    };
}
