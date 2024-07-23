create table dbo.PlayerTotal(
    AccountId int not null,
    Field nvarchar(255) not null,
    [Count] int not null,
    [Sum] float not null,
    constraint PK_Total primary key (AccountId,Field)
)
go

alter table [dbo].[PlayerTotal] with check add constraint [FK_PlayerTotal_Player] foreign key ([AccountId]) references [dbo].[Player] ([AccountId])
go

create table Staging.PlayerTotal(
    AccountId int not null,
    Field nvarchar(255) not null,
    [Count] int not null,
    [Sum] float not null,
    constraint PK_Total primary key (AccountId,Field)
)
go
