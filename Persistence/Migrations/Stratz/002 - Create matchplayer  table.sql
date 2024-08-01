﻿create table Stratz.MatchPlayer(
    MatchId bigint not null,
    PlayerSlot int null,
    HeroId int null,
    SteamAccountId int not null,
    IsRadiant bit null,
    NumKills int null,
    NumDeaths int null,
    NumAssists int null,
    LeaverStatus int null,
    NumLastHits int null,
    NumDenies int null,
    GoldPerMinute int null,
    ExperiencePerMinute int null,
    Level int null,
    Gold int null,
    GoldSpent int null,
    HeroDamage int null,
    TowerDamage int null,
    PartyId int null,
    Item0Id int null,
    Item1Id int null,
    Item2Id int null,
    Item3Id int null,
    Item4Id int null,
    Item5Id int null,
    HeroHealing int null,
    IsVictory bit null,
    Networth int null,
    Neutral0Id int null,
    Variant int null,
    IsRandom bit null,
    Lane int null,
    IntentionalFeeding bit null,
    Role int null,
    Imp float null,
    Award int null,
    Behavior int null,
    RoamLane int null,
    DotaPlusHeroXp int null,
    InvisibleSeconds int null,
    constraint PK_Stratz_MatchPlayer primary key (MatchId,SteamAccountId)
)
go

create table StratzStaging.MatchPlayer(
    MatchId bigint not null,
    PlayerSlot int null,
    HeroId int null,
    SteamAccountId int not null,
    IsRadiant bit null,
    NumKills int null,
    NumDeaths int null,
    NumAssists int null,
    LeaverStatus int null,
    NumLastHits int null,
    NumDenies int null,
    GoldPerMinute int null,
    ExperiencePerMinute int null,
    Level int null,
    Gold int null,
    GoldSpent int null,
    HeroDamage int null,
    TowerDamage int null,
    PartyId int null,
    Item0Id int null,
    Item1Id int null,
    Item2Id int null,
    Item3Id int null,
    Item4Id int null,
    Item5Id int null,
    HeroHealing int null,
    IsVictory bit null,
    Networth int null,
    Neutral0Id int null,
    Variant int null,
    IsRandom bit null,
    Lane int null,
    IntentionalFeeding bit null,
    Role int null,
    Imp float null,
    Award int null,
    Behavior int null,
    RoamLane int null,
    DotaPlusHeroXp int null,
    InvisibleSeconds int null,
    constraint PK_Stratz_Staging_MatchPlayer primary key (MatchId,SteamAccountId)
)
go
