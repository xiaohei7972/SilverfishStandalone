using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 南海船工
// Southsea Deckhand
// 如果你装备着武器，本随从拥有 冲锋。
// Has Charge while you have a weapon equipped.
[CardSim("CS2_146")]
internal static class Sim_CS2_146
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [new(AuraRecipients.Self) { Condition = AuraCondition.WhileWeaponEquipped, GrantedTag = GAME_TAG.CHARGE }]
    };
}
