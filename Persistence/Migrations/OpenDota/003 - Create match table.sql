create table OpenDota.Match(
    MatchId bigint not null,
	RadiantWin bit null,
	Duration int null,
	PreGameDuration int null,
	StartTime int null,
	MatchSeqNum bigint null,
	TowerStatusRadiant int null,
	TowerStatusDire int null,
	BarracksStatusRadiant int null,
	BarracksStatusDire int null,
	Cluster int null,
	FirstBloodTime int null,
	LobbyType int null,
	HumanPlayers int null,
	LeagueId int null,
	GameMode int null,
	Flags int null,
	Engine int null,
	RadiantScore int null,
	DireScore int null,
	Patch int null,
	Region int null,
    constraint PK_OpenDota_Match primary key (MatchId)
)
go

alter table [OpenDota].[Match] with check add constraint [FK_OpenDota_Match_GameMode] foreign key ([GameMode]) references [Reference].[GameMode] ([Id])
go

alter table [OpenDota].[Match] with check add constraint [FK_OpenDota_Match_LobbyType] foreign key ([LobbyType]) references [Reference].[LobbyType] ([Id])
go

create table OpenDotaStaging.Match(
    MatchId bigint not null,
	RadiantWin bit null,
	Duration int null,
	PreGameDuration int null,
	StartTime int null,
	MatchSeqNum bigint null,
	TowerStatusRadiant int null,
	TowerStatusDire int null,
	BarracksStatusRadiant int null,
	BarracksStatusDire int null,
	Cluster int null,
	FirstBloodTime int null,
	LobbyType int null,
	HumanPlayers int null,
	LeagueId int null,
	GameMode int null,
	Flags int null,
	Engine int null,
	RadiantScore int null,
	DireScore int null,
	Patch int null,
	Region int null,
    constraint PK_OpenDota_Staging_Match primary key (MatchId)
)
go
