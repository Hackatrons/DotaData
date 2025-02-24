﻿create table Stratz.Match(
    Id bigint not null,
    DidRadiantWin bit null,
    DurationSeconds int null,
    StartDateTime int null,
    ClusterId int null,
    FirstBloodTime int null,
    LobbyType int null,
    NumHumanPlayers int null,
    GameMode int null,
    IsStats bit null,
    GameVersionId int null,
    RegionId int null,
    SequenceNum bigint null,
    Rank int null,
    Bracket int null,
    EndDateTime int null,
    DidRequestDownload bit null,
    AvgImp float null,
    ParsedDateTime int null,
    StatsDateTime int null,
    AnalysisOutcome int null,
    PredictedOutcomeWeight int null,
    BottomLaneOutcome int null,
    MidLaneOutcome int null,
    TopLaneOutcome int null,
    RadiantNetworthLead varchar(1024) null,
    RadiantExperienceLead varchar(1024) null,
    RadiantKills varchar(1024) null,
    DireKills varchar(1024) null,
    WinRates varchar(1024) null,
    PredictedWinRates varchar(1024) null,
    constraint PK_Stratz_Match primary key (Id)
)
go

alter table [Stratz].[Match] with check add constraint [FK_Stratz_Match_GameMode] foreign key ([GameMode]) references [Reference].[GameMode] ([Id])
go

alter table [Stratz].[Match] with check add constraint [FK_Stratz_Match_LobbyType] foreign key ([LobbyType]) references [Reference].[LobbyType] ([Id])
go

create table StratzStaging.Match(
    Id bigint not null,
    DidRadiantWin bit null,
    DurationSeconds int null,
    StartDateTime int null,
    ClusterId int null,
    FirstBloodTime int null,
    LobbyType int null,
    NumHumanPlayers int null,
    GameMode int null,
    IsStats bit null,
    GameVersionId int null,
    RegionId int null,
    SequenceNum bigint null,
    Rank int null,
    Bracket int null,
    EndDateTime int null,
    DidRequestDownload bit null,
    AvgImp float null,
    ParsedDateTime int null,
    StatsDateTime int null,
    AnalysisOutcome int null,
    PredictedOutcomeWeight int null,
    BottomLaneOutcome int null,
    MidLaneOutcome int null,
    TopLaneOutcome int null,
    RadiantNetworthLead varchar(1024) null,
    RadiantExperienceLead varchar(1024) null,
    RadiantKills varchar(1024) null,
    DireKills varchar(1024) null,
    WinRates varchar(1024) null,
    PredictedWinRates varchar(1024) null,    
    constraint PK_Stratz_Staging_Match primary key (Id)
)
go
