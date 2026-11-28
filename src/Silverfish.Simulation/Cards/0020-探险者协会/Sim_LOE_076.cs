namespace Silverfish.Simulation.Cards;

// 芬利·莫格顿爵士
// Sir Finley Mrrgglton
// 战吼：发现一个新的基础英雄技能。
// Battlecry: Discover a new basic Hero Power.
[CardSim("LOE_076")]
internal static class Sim_LOE_076
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice() { Options = ["HERO_01bp", "HERO_02bp", "HERO_03bp", "HERO_04bp", "HERO_05bp", "HERO_06bp", "HERO_07bp", "HERO_08bp", "HERO_09bp", "HERO_10bp", "HERO_11bp"], ReplaceHeroPower = true },
    };
}
