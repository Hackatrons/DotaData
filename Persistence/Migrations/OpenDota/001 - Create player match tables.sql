create table OpenDota.PlayerMatch(
    AccountId int not null,
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
    constraint PK_OpenDota_PlayerMatch primary key (AccountId,MatchId)
)

alter table [OpenDota].[PlayerMatch] with check add constraint [FK_OpenDota_PlayerMatch_GameMode] foreign key ([GameMode]) references [Reference].[GameMode] ([Id])
go

alter table [OpenDota].[PlayerMatch] with check add constraint [FK_OpenDota_PlayerMatch_Hero] foreign key ([HeroId]) references [Reference].[Hero] ([Id])
go

alter table [OpenDota].[PlayerMatch] with check add constraint [FK_OpenDota_PlayerMatch_LobbyType] foreign key ([LobbyType]) references [Reference].[LobbyType] ([Id])
go

alter table [OpenDota].[PlayerMatch] with check add constraint [FK_OpenDota_PlayerMatch_Player] foreign key ([AccountId]) references [OpenDota].[Player] ([AccountId])
go

create table OpenDotaStaging.PlayerMatch(
    AccountId int not null,
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
    constraint PK_Staging_PlayerMatch primary key (AccountId,MatchId)
)