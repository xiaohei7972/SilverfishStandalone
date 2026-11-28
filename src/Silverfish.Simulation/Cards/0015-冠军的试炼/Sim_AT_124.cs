namespace Silverfish.Simulation.Cards;

// 博尔夫·碎盾
// Bolf Ramshield
// 每当你的英雄受到伤害，改为由本随从承担。
// Whenever your hero takes damage, this minion takes it instead.
[CardSim("AT_124")]
internal static class Sim_AT_124
{
    internal static CardRules Rules { get; } = new()
    {
        RedirectFriendlyHeroDamageToSelf = true,
    };
}
