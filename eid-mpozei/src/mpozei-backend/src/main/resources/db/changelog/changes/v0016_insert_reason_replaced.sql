alter table mpozei.reason_nomenclature
add column if not exists internal boolean default false;

insert into mpozei.reason_nomenclature(id, create_date, created_by, last_update, updated_by, active, description, language, name, nomenclature_type, internal)
values
    ('38a2f501-2142-4d82-bb61-7a29ce4202a8', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Replaced', 'EN', 'REPLACED', '5dfa852a-0094-43a6-be10-39e26aeb9232', true),
    ('c2492015-1cc4-45aa-80b3-57314e631f91', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Заменен', 'BG', 'REPLACED', '5dfa852a-0094-43a6-be10-39e26aeb9232', true);

update mpozei.reason_nomenclature
set internal = true
where name = 'DENIED_TIMED_OUT';