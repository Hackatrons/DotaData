-- import tables
create table Raw.PlayerTotal(
    PlayerId int not null,
    Field nvarchar(255) not null,
    [Count] int not null,
    [Sum] float not null,
    constraint PK_Total primary key (PlayerId,Field)
)
go

-- staging tables
create table Staging.PlayerTotal(
    PlayerId int not null,
    Field nvarchar(255) not null,
    [Count] int not null,
    [Sum] float not null,
    constraint PK_Total primary key (PlayerId,Field)
)
go
