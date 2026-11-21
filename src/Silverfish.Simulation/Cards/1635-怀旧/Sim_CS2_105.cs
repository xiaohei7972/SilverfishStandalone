using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 英勇打击
// Heroic Strike
// 在本回合中，使你的英雄获得+4攻击力。
// Give your hero +4_Attack this turn.
[CardSim("CS2_105")]
internal static class Sim_CS2_105
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_CS2_105e.Rule.ApplyTo(StatTarget.FriendlyHero)]
    };
}
