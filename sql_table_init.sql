create type element as enum ('fire', 'water', 'normal');

alter type element owner to postgres;

create type species as enum ('goblin', 'drache', 'zauberer', 'ork', 'ritter', 'kraken', 'elfe');

alter type species owner to postgres;

create type card_type as enum ('monster', 'spell');

alter type card_type owner to postgres;

create table users
(
    username varchar             not null,
    password varchar             not null,
    token    varchar             not null,
    user_id  serial
        constraint users_pk
            primary key,
    image    text,
    bio      text,
    name     text,
    coins    integer default 20  not null,
    wins     integer default 0   not null,
    losses   integer default 0   not null,
    draws    integer default 0   not null,
    elo      integer default 100 not null
);

alter table users
    owner to postgres;

create unique index users_user_id_uindex
    on users (user_id);

create unique index users_token_uindex
    on users (token);

create unique index users_username_uindex
    on users (username);

create table cards
(
    card_id text    not null
        constraint cards_pk
            primary key,
    name    text    not null,
    dmg     integer not null,
    token   text
        constraint cards_users_token_fk
            references users (token)
            on update cascade on delete cascade
            deferrable initially deferred
);

alter table cards
    owner to postgres;

create table packages
(
    package_id serial
        primary key,
    card1_id   text not null
        references cards
            on update cascade on delete cascade,
    card2_id   text not null
        references cards
            on update cascade on delete cascade,
    card3_id   text not null
        references cards
            on update cascade on delete cascade,
    card4_id   text not null
        references cards
            on update cascade on delete cascade,
    card5_id   text not null
        references cards
            on update cascade on delete cascade,
    owner      text
        references users (token)
            on update cascade on delete cascade
);

alter table packages
    owner to postgres;

create unique index packages_card1_uindex
    on packages (card1_id);

create unique index packages_card2_uindex
    on packages (card2_id);

create unique index packages_card3_uindex
    on packages (card3_id);

create unique index packages_card4_uindex
    on packages (card4_id);

create unique index packages_card5_uindex
    on packages (card5_id);

create table decks
(
    deck_id  serial
        constraint decks_pk
            primary key,
    token    text not null
        constraint decks_users_token_fk
            references users (token)
            on update cascade on delete cascade,
    card1_id text
        constraint decks_cards_card_id_fk
            references cards
            on update cascade on delete cascade,
    card2_id text
        constraint decks_cards_card_id_fk_2
            references cards
            on update cascade on delete cascade,
    card3_id text
        constraint decks_cards_card_id_fk_3
            references cards
            on update cascade on delete cascade,
    card4_id text
        constraint decks_cards_card_id_fk_4
            references cards
            on update cascade on delete cascade
);

alter table decks
    owner to postgres;

create unique index decks_card1_id_uindex
    on decks (card1_id);

create unique index decks_card2_id_uindex
    on decks (card2_id);

create unique index decks_card3_id_uindex
    on decks (card3_id);

create unique index decks_card4_id_uindex
    on decks (card4_id);

create unique index decks_deck_id_uindex
    on decks (deck_id);

create unique index decks_user_id_uindex
    on decks (token);

create table trading
(
    trading_id  text not null
        constraint trading_pk
            primary key,
    cardtotrade text not null
        constraint trading_cards_card_id_fk
            references cards
            on update cascade on delete cascade,
    mindmg      integer,
    element     element,
    cardtype    card_type,
    species     species,
    usertoken   text not null
        constraint trading_users_token_fk
            references users (token)
            on update cascade on delete cascade
);

alter table trading
    owner to postgres;

create unique index trading_cardtotrade_uindex
    on trading (cardtotrade);

create unique index trading_trading_id_uindex
    on trading (trading_id);

