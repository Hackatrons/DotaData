create table dbo.Hero(
    Id int not null,
    Name nvarchar(255) not null,
    LocalizedName nvarchar(255) not null,
    PrimaryAttr varchar(10) not null,
    AttackType varchar(10) not null,
    Legs int not null,
    constraint PK_Hero primary key (Id)
)