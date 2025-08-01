insert into mpozei.nomenclature_type (id, name, create_date, created_by, last_update, updated_by)
values ('568fb1ec-d894-4202-957e-0caf3a506a59', 'RESUME_REASON_TYPE', now(), 'SYSTEM', now(), 'SYSTEM');

insert into mpozei.reason_nomenclature(id, create_date, created_by, last_update, updated_by, active, description, language, name, nomenclature_type, internal)
values
    ('f915e73f-930f-4fcf-a0a7-cd7fd36d1433', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Resumed by NAIF', 'EN', 'RESUMED_BY_NAIF', '568fb1ec-d894-4202-957e-0caf3a506a59', true),
    ('1ad967ec-cce2-4ac2-8d8a-a52e92b38ede', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Възобновен от НАИФ', 'BG', 'RESUMED_BY_NAIF', '568fb1ec-d894-4202-957e-0caf3a506a59', true),
    ('13c6c0fb-a1c7-47ac-931a-bdc149cae0ca', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Revoked by NAIF', 'EN', 'REVOKED_BY_NAIF', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e', true),
    ('b469afcd-765e-42dd-a745-3577b7e12385', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Прекратен от НАИФ', 'BG', 'REVOKED_BY_NAIF', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e', true),
    ('bd7042bd-6dc2-4c88-92ef-cc0a477b4598', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Stopped by NAIF', 'EN', 'STOPPED_BY_NAIF', '5dfa852a-0094-43a6-be10-39e26aeb9232', true),
    ('515e274a-ab10-43ee-9249-5ea11678c765', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Спрян от НАИФ', 'BG', 'STOPPED_BY_NAIF', '5dfa852a-0094-43a6-be10-39e26aeb9232', true);