create table PlayerMatch(
    PlayerId int not null,
    MatchId bigint not null,
    constraint PK_PlayerMatch primary key (PlayerId, MatchId)
)