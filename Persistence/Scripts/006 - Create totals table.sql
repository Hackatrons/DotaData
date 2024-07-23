-- import tables
create table Raw.PlayerTotal(
    AccountId int not null,
    Field nvarchar(255) not null,
    [Count] int not null,
    [Sum] float not null,
    constraint PK_Total primary key (AccountId,Field)
)
go

-- staging tables
create table Staging.PlayerTotal(
    AccountId int not null,
    Field nvarchar(255) not null,
    [Count] int not null,
    [Sum] float not null,
    constraint PK_Total primary key (AccountId,Field)
)
go
