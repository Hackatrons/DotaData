create table Raw.LobbyType(
    Id int not null,
    Name nvarchar(255) not null,
    Balanced bit null,
    constraint PK_LobbyType primary key (Id)
)
go

insert into Raw.LobbyType(Id, Name, Balanced)
values (0, 'lobby_type_normal', 1)

insert into Raw.LobbyType(Id, Name, Balanced)
values (1, 'lobby_type_practice', 1)

insert into Raw.LobbyType(Id, Name, Balanced)
values (2, 'lobby_type_tournament', 1)

insert into Raw.LobbyType(Id, Name)
values (3, 'lobby_type_tutorial')

insert into Raw.LobbyType(Id, Name)
values (4, 'lobby_type_coop_bots')

insert into Raw.LobbyType(Id, Name, Balanced)
values (5, 'lobby_type_ranked_team_mm', 1)

insert into Raw.LobbyType(Id, Name, Balanced)
values (6, 'lobby_type_ranked_solo_mm', 1)

insert into Raw.LobbyType(Id, Name, Balanced)
values (7, 'lobby_type_ranked', 1)

insert into Raw.LobbyType(Id, Name)
values (8, 'lobby_type_1v1_mid')

insert into Raw.LobbyType(Id, Name, Balanced)
values (9, 'lobby_type_battle_cup', 1)

insert into Raw.LobbyType(Id, Name, Balanced)
values (10, 'lobby_type_local_bots', 1)

insert into Raw.LobbyType(Id, Name, Balanced)
values (11, 'lobby_type_spectator', 1)

insert into Raw.LobbyType(Id, Name)
values (12, 'lobby_type_event')

insert into Raw.LobbyType(Id, Name, Balanced)
values (13, 'lobby_type_gauntlet', 1)

insert into Raw.LobbyType(Id, Name, Balanced)
values (14, 'lobby_type_new_player', 1)

insert into Raw.LobbyType(Id, Name, Balanced)
values (15, 'lobby_type_featured', 1)