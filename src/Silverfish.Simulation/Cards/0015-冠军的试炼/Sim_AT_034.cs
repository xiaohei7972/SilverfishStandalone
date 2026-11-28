using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 淬毒利刃
// Poisoned Blade
// 你的英雄技能不会取代该武器，改为+1攻击力。
// Your Hero Power gives this +1_Attack instead of replacing it.
[CardSim("AT_034")]
internal static class Sim_AT_034
{
    internal static CardRules Rules { get; } = new()
    {
        ReplaceHeroPowerEquipEffect = Sim_AT_034e.Rule.ApplyToWeapon(),
    };
}
