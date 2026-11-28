namespace Silverfish.Simulation.Cards;

// 生命分流
// Life Tap
// 英雄技能 抽一张牌并受到$2点伤害。
// Hero Power Draw a card and take $2_damage.
[CardSim("HERO_07bp")]
internal static class Sim_HERO_07bp
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DrawEffect(1),
            new DamageEffect(DamageTarget.FriendlyHero, 2),
        ],
    };
}
