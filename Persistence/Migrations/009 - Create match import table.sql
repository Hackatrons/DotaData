create table OpenDota.MatchImport(
    MatchId bigint not null,
	Success bit not null,
	ErrorMessage nvarchar(1024) null,
	ErrorCode int null,
	Timestamp datetime2(7) not null default getutcdate(),
)
go
