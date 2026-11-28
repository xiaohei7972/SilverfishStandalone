using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 熔岩震击
// Lava Shock
// 造成$2点伤害。 将你所有过载的法力水晶解锁。
// Deal $2 damage. Unlock your Overloaded Mana Crystals.
[CardSim("BRM_011")]
internal static class Sim_BRM_011
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.Selected, 2), new UnlockOverloadEffect()]
    };
}
