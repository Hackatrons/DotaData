create table OpenDota.MatchImport(
    MatchId bigint not null,
	Success bit not null,
	ErrorMessage nvarchar(1024) null,
	ErrorCode int null,
    constraint PK_OpenDota_MatchImport primary key (MatchId)
)
go
