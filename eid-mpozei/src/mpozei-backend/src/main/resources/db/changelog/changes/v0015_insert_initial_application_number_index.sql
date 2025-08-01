create table if not exists mpozei.number_generator
(
    id uuid not null primary key,
    counter integer
);

insert into mpozei.number_generator (id, counter) values ('eae8db59-b348-43e5-b21f-c9c9d0d65d9b', 0);