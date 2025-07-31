insert into mpozei.nomenclature_type (id, name, create_date, created_by, last_update, updated_by)
values ('5dfa852a-0094-43a6-be10-39e26aeb9232', 'REVOKE_REASON_TYPE', now(), 'SYSTEM', now(), 'SYSTEM');

insert into mpozei.reason_nomenclature(id, create_date, created_by, last_update, updated_by, active, description, language, name, nomenclature_type)
values
    ('c8acd17c-5d13-4c04-9589-f54b95e235ec', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Revoked by administrator', 'EN', 'REVOKED_BY_ADMINISTRATOR', '5dfa852a-0094-43a6-be10-39e26aeb9232'),
    ('0c57d265-bb46-4427-80fa-3bfae5df7066', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Прекратено от администратор', 'BG', 'REVOKED_BY_ADMINISTRATOR', '5dfa852a-0094-43a6-be10-39e26aeb9232'),
    ('a6b9f194-efd8-4521-8ccd-d2f139935623', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Other', 'EN', 'REVOKED_OTHER', '5dfa852a-0094-43a6-be10-39e26aeb9232'),
    ('6f27c323-438e-454a-b4c0-45720e29c4fd', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Друго', 'BG', 'REVOKED_OTHER', '5dfa852a-0094-43a6-be10-39e26aeb9232');