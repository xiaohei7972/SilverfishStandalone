using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

internal static class CardTraitReader
{
    // 客户端 CardRace 映射。第一列是 CARDRACE 的值，第二列是独立种族标签。
    // 附加标签为零时只使用 CARDRACE 判断。
    private static readonly (TAG_RACE Race, int Tag, MinionType Type)[] RaceTags =
    [
        (TAG_RACE.BLOODELF, 2524, MinionType.BloodElf), (TAG_RACE.DRAENEI, 2525, MinionType.Draenei), (TAG_RACE.DWARF, 2526, MinionType.Dwarf),
        (TAG_RACE.GNOME, 2527, MinionType.Gnome), (TAG_RACE.GOBLIN, 2528, MinionType.Goblin), (TAG_RACE.HUMAN, 2529, MinionType.Human),
        (TAG_RACE.NIGHTELF, 2530, MinionType.NightElf), (TAG_RACE.ORC, 2531, MinionType.Orc), (TAG_RACE.TAUREN, 2532, MinionType.Tauren),
        (TAG_RACE.TROLL, 2533, MinionType.Troll), (TAG_RACE.UNDEAD, 2534, MinionType.Undead), (TAG_RACE.WORGEN, 2535, MinionType.Worgen),
        (TAG_RACE.GOBLIN2, 0, MinionType.Goblin2), (TAG_RACE.MURLOC, 2536, MinionType.Murloc), (TAG_RACE.DEMON, 2537, MinionType.Demon),
        (TAG_RACE.SCOURGE, 2538, MinionType.Scourge), (TAG_RACE.MECHANICAL, 2539, MinionType.Mech), (TAG_RACE.ELEMENTAL, 2540, MinionType.Elemental),
        (TAG_RACE.OGRE, 2541, MinionType.Ogre), (TAG_RACE.BEAST, 2542, MinionType.Beast), (TAG_RACE.TOTEM, 2543, MinionType.Totem),
        (TAG_RACE.NERUBIAN, 2544, MinionType.Nerubian), (TAG_RACE.PIRATE, 2522, MinionType.Pirate), (TAG_RACE.DRAGON, 2523, MinionType.Dragon),
        (TAG_RACE.BLANK, 0, MinionType.Blank), (TAG_RACE.EGG, 2545, MinionType.Egg), (TAG_RACE.QUILBOAR, 2546, MinionType.Quilboar),
        (TAG_RACE.CENTAUR, 2547, MinionType.Centaur), (TAG_RACE.FURBOLG, 2548, MinionType.Furbolg), (TAG_RACE.HIGHELF, 2549, MinionType.HighElf),
        (TAG_RACE.TREANT, 2550, MinionType.Treant), (TAG_RACE.OWLKIN, 2551, MinionType.Owlkin), (TAG_RACE.HALFORC, 2552, MinionType.HalfOrc),
        (TAG_RACE.LOCK, 0, MinionType.Lock), (TAG_RACE.NAGA, 2553, MinionType.Naga), (TAG_RACE.OLDGOD, 2554, MinionType.OldGod),
        (TAG_RACE.PANDAREN, 2555, MinionType.Pandaren), (TAG_RACE.GRONN, 2556, MinionType.Gronn), (TAG_RACE.CELESTIAL, 2584, MinionType.Celestial),
        (TAG_RACE.GNOLL, 2585, MinionType.Gnoll), (TAG_RACE.GOLEM, 2586, MinionType.Golem), (TAG_RACE.HARPY, 2587, MinionType.Harpy),
        (TAG_RACE.VULPERA, 2588, MinionType.Vulpera), (TAG_RACE.ABERRATION, 4756, MinionType.Aberration)
    ];

    internal static MinionType? ReadMinionTypes(CardMetadata metadata)
    {
        var primary = metadata.ClientRace;
        if (primary == TAG_RACE.ALL)
            return MinionType.All;

        var result = MinionType.None;
        foreach (var (race, tag, type) in RaceTags)
            if (race == primary || (tag != 0 && metadata.GetTag(tag) > 0))
                result |= type;
        return result == MinionType.None ? null : result;
    }

    internal static SpellSchool? ReadSpellSchool(CardMetadata metadata)
    {
        var school = (SpellSchool)metadata.ClientSpellSchool;
        return school != SpellSchool.None && Enum.IsDefined(school) ? school : null;
    }
}
