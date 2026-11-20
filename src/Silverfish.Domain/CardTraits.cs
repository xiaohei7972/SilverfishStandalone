using Silverfish.Domain.Client;

namespace Silverfish.Domain;

/// <summary>随从种族。双种族用两个标志位表示。</summary>
[Flags]
public enum MinionType : ulong
{
    None = 0,
    BloodElf = 1UL << 0,
    Draenei = 1UL << 1,
    Dwarf = 1UL << 2,
    Gnome = 1UL << 3,
    Goblin = 1UL << 4,
    Human = 1UL << 5,
    NightElf = 1UL << 6,
    Orc = 1UL << 7,
    Tauren = 1UL << 8,
    Troll = 1UL << 9,
    Undead = 1UL << 10,
    Worgen = 1UL << 11,
    Goblin2 = 1UL << 12,
    Murloc = 1UL << 13,
    Demon = 1UL << 14,
    Scourge = 1UL << 15,
    Mech = 1UL << 16,
    Elemental = 1UL << 17,
    Ogre = 1UL << 18,
    Beast = 1UL << 19,
    Totem = 1UL << 20,
    Nerubian = 1UL << 21,
    Pirate = 1UL << 22,
    Dragon = 1UL << 23,
    Blank = 1UL << 24,
    All = 1UL << 25,
    Egg = 1UL << 26,
    Quilboar = 1UL << 27,
    Centaur = 1UL << 28,
    Furbolg = 1UL << 29,
    HighElf = 1UL << 30,
    Treant = 1UL << 31,
    Owlkin = 1UL << 32,
    HalfOrc = 1UL << 33,
    Lock = 1UL << 34,
    Naga = 1UL << 35,
    OldGod = 1UL << 36,
    Pandaren = 1UL << 37,
    Gronn = 1UL << 38,
    Celestial = 1UL << 39,
    Gnoll = 1UL << 40,
    Golem = 1UL << 41,
    Harpy = 1UL << 42,
    Vulpera = 1UL << 43,
    Aberration = 1UL << 44
}

/// <summary>卡牌的派系值与 CardDefs 的 SPELL_SCHOOL 标签一致。</summary>
public enum SpellSchool
{
    None = (int)TAG_SPELL_SCHOOL.NONE,
    Arcane = (int)TAG_SPELL_SCHOOL.ARCANE,
    Fire = (int)TAG_SPELL_SCHOOL.FIRE,
    Frost = (int)TAG_SPELL_SCHOOL.FROST,
    Nature = (int)TAG_SPELL_SCHOOL.NATURE,
    Holy = (int)TAG_SPELL_SCHOOL.HOLY,
    Shadow = (int)TAG_SPELL_SCHOOL.SHADOW,
    Fel = (int)TAG_SPELL_SCHOOL.FEL,
    PhysicalCombat = (int)TAG_SPELL_SCHOOL.PHYSICAL_COMBAT,
    Tavern = (int)TAG_SPELL_SCHOOL.TAVERN,
    Spellcraft = (int)TAG_SPELL_SCHOOL.SPELLCRAFT,
    LesserTrinket = (int)TAG_SPELL_SCHOOL.LESSER_TRINKET,
    GreaterTrinket = (int)TAG_SPELL_SCHOOL.GREATER_TRINKET,
    Upgrade = (int)TAG_SPELL_SCHOOL.UPGRADE
}
