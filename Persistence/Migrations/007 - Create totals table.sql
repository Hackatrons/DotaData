create table OpenDota.PlayerTotal(
    AccountId int not null,
    Field nvarchar(255) not null,
    [Count] int not null,
    [Sum] float not null,
    constraint PK_OpenDota_PlayerTotal primary key (AccountId,Field)
)
go

alter table [OpenDota].[PlayerTotal] with check add constraint [FK_OpenDota_PlayerTotal_Player] foreign key ([AccountId]) references [OpenDota].[Player] ([AccountId])
go

create table OpenDotaStaging.PlayerTotal(
    AccountId int not null,
    Field nvarchar(255) not null,
    [Count] int not null,
    [Sum] float not null,
    constraint PK_OpenDotaStaging_PlayerTotal primary key (AccountId,Field)
)
go
