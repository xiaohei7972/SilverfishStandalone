namespace Silverfish.Simulation.Cards;

// 狂乱传染
// Spreading Madness
// 造成$13点伤害，随机分配到所有角色身上。
// Deal $13 damage randomly split among ALL characters.
[CardSim("OG_116")]
internal static class Sim_OG_116
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new RandomDamageEffect(RandomCharacterPool.AllCharacters, 1, 13),
        ],
    };
}
