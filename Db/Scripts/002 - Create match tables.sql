-- import tables
create table Raw.Match(
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
    constraint PK_Match primary key (MatchId)
)

create table Raw.PlayerMatch(
    PlayerId int not null,
    MatchId bigint not null,
    constraint PK_PlayerMatch primary key (PlayerId, MatchId)
)

-- staging tables
create table Staging.Match(
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
    constraint PK_Match primary key (MatchId)
)

create table Staging.PlayerMatch(
    PlayerId int not null,
    MatchId bigint not null,
    constraint PK_PlayerMatch primary key (PlayerId, MatchId)
)