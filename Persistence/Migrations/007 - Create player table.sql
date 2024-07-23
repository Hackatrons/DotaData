-- import tables
create table Raw.Player(
    AccountId int not null,
    PersonaName nvarchar(255) null,
    Name nvarchar(255) null,
    Plus bit null,
    Cheese int null,
    SteamId nvarchar(255) null,
    Avatar nvarchar(1024) null,
    AvatarMedium nvarchar(1024) null,
    AvatarFull nvarchar(1024) null,
    ProfileUrl nvarchar(1024) null,
    LastLogin int null,
    LocCountryCode varchar(10) null,
    Status varchar(255) null,
    FhUnavailable bit null,
    IsContributor bit null,
    IsSubscriber bit null,
    RankTier int null,
    SoloCompetitiveRank int null,
    CompetitiveRank int null,
    LeaderboardRank int null,
    constraint PK_Player primary key (AccountId)
)
go

-- staging tables
create table Staging.Player(
    AccountId int not null,
    PersonaName nvarchar(255) null,
    Name nvarchar(255) null,
    Plus bit null,
    Cheese int null,
    SteamId nvarchar(255) null,
    Avatar nvarchar(1024) null,
    AvatarMedium nvarchar(1024) null,
    AvatarFull nvarchar(1024) null,
    ProfileUrl nvarchar(1024) null,
    LastLogin int null,
    LocCountryCode varchar(10) null,
    Status varchar(255) null,
    FhUnavailable bit null,
    IsContributor bit null,
    IsSubscriber bit null,
    RankTier int null,
    SoloCompetitiveRank int null,
    CompetitiveRank int null,
    LeaderboardRank int null,
    constraint PK_Player primary key (AccountId)
)
go
