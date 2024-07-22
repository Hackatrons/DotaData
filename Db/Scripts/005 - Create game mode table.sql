create table Raw.GameMode(
    Id int not null,
    Name nvarchar(255) not null,
    Balanced bit null,
    constraint PK_GameMode primary key (Id)
)
go

insert into Raw.GameMode(Id, Name, Balanced)
values (0, 'game_mode_unknown', 1)

insert into Raw.GameMode(Id, Name, Balanced)
values (1, 'game_mode_all_pick', 1)

insert into Raw.GameMode(Id, Name, Balanced)
values (2, 'game_mode_captains_mode', 1)

insert into Raw.GameMode(Id, Name, Balanced)
values (3, 'game_mode_random_draft', 1)

insert into Raw.GameMode(Id, Name, Balanced)
values (4, 'game_mode_single_draft', 1)

insert into Raw.GameMode(Id, Name, Balanced)
values (5, 'game_mode_all_random', 1)

insert into Raw.GameMode(Id, Name)
values (6, 'game_mode_intro')

insert into Raw.GameMode(Id, Name)
values (7, 'game_mode_diretide')

insert into Raw.GameMode(Id, Name)
values (8, 'game_mode_reverse_captains_mode')

insert into Raw.GameMode(Id, Name)
values (9, 'game_mode_greeviling')

insert into Raw.GameMode(Id, Name)
values (10, 'game_mode_tutorial')

insert into Raw.GameMode(Id, Name)
values (11, 'game_mode_mid_only')

insert into Raw.GameMode(Id, Name, Balanced)
values (12, 'game_mode_least_played', 1)

insert into Raw.GameMode(Id, Name)
values (13, 'game_mode_limited_heroes')

insert into Raw.GameMode(Id, Name)
values (14, 'game_mode_compendium_matchmaking')

insert into Raw.GameMode(Id, Name)
values (15, 'game_mode_custom')

insert into Raw.GameMode(Id, Name, Balanced)
values (16, 'game_mode_captains_draft', 1)

insert into Raw.GameMode(Id, Name, Balanced)
values (17, 'game_mode_balanced_draft', 1)

insert into Raw.GameMode(Id, Name)
values (18, 'game_mode_ability_draft')

insert into Raw.GameMode(Id, Name)
values (19, 'game_mode_event')

insert into Raw.GameMode(Id, Name)
values (20, 'game_mode_all_random_death_match')

insert into Raw.GameMode(Id, Name)
values (21, 'game_mode_1v1_mid')

insert into Raw.GameMode(Id, Name, Balanced)
values (22, 'game_mode_all_draft', 1)

insert into Raw.GameMode(Id, Name)
values (23, 'game_mode_turbo')

insert into Raw.GameMode(Id, Name)
values (24, 'game_mode_mutation')

insert into Raw.GameMode(Id, Name)
values (25, 'game_mode_coaches_challenge')
