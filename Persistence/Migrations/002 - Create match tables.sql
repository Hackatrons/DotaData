﻿-- import tables
create table Raw.PlayerMatch(
    AccountId bigint not null,
    MatchId bigint not null,
    PlayerSlot int null,
    RadiantWin bit null,
    GameMode int null,
    HeroId int null,
    StartTime int null,
    Duration int null,
    LobbyType int null,
    Version int null,
    Kills int null,
    Deaths int null,
    Assists int null,
    AverageRank int null,
    LeaverStatus int null,
    PartySize int null,
    HeroVariant int null,
    constraint PK_Match primary key (AccountId,MatchId)
)

-- staging tables
create table Staging.PlayerMatch(
    AccountId bigint not null,
    MatchId bigint not null,
    PlayerSlot int null,
    RadiantWin bit null,
    GameMode int null,
    HeroId int null,
    StartTime int null,
    Duration int null,
    LobbyType int null,
    Version int null,
    Kills int null,
    Deaths int null,
    Assists int null,
    AverageRank int null,
    LeaverStatus int null,
    PartySize int null,
    HeroVariant int null,
    constraint PK_Match primary key (AccountId,MatchId)
)