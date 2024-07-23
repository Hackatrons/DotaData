create table dbo.PlayerMatch(
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
    constraint PK_PlayerMatch primary key (AccountId,MatchId)
)

alter table [dbo].[PlayerMatch] with check add constraint [FK_PlayerMatch_GameMode] foreign key ([GameMode]) references [dbo].[GameMode] ([Id])
go

alter table [dbo].[PlayerMatch] with check add constraint [FK_PlayerMatch_Hero] foreign key ([HeroId]) references [dbo].[Hero] ([Id])
go

alter table [dbo].[PlayerMatch] with check add constraint [FK_PlayerMatch_LobbyType] foreign key ([LobbyType]) references  [dbo].[LobbyType] ([Id])
go

alter table [dbo].[PlayerMatch] with check add constraint [FK_PlayerMatch_Player] foreign key ([AccountId]) references [dbo].[Player] ([AccountId])
go

create table Staging.PlayerMatch(
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