using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 弃暗投明
// Renounce Darkness
// 将你的英雄技能和术士卡牌替换成另一职业的。这些牌的法力值消耗减少（1）点。
// Replace your Hero Power and Warlock cards with another class's. The cards cost (1) less.
[CardSim("OG_118")]
internal static class Sim_OG_118
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ReplaceClassCardsEffect(TAG_CLASS.WARLOCK,
            [
                new(TAG_CLASS.WARRIOR, "HERO_01bp"),
                new(TAG_CLASS.SHAMAN, "HERO_02bp"),
                new(TAG_CLASS.ROGUE, "HERO_03bp"),
                new(TAG_CLASS.PALADIN, "HERO_04bp"),
                new(TAG_CLASS.HUNTER, "HERO_05bp"),
                new(TAG_CLASS.DRUID, "HERO_06bp"),
                new(TAG_CLASS.MAGE, "HERO_08bp"),
                new(TAG_CLASS.PRIEST, "HERO_09bp"),
                new(TAG_CLASS.DEMONHUNTER, "HERO_10bp"),
                new(TAG_CLASS.DEATHKNIGHT, "HERO_11bp"),
            ], -1, Sim_OG_118f.CardId),
        ],
    };
}
