insert into mpozei.nomenclature_type (id, name, create_date, created_by, last_update, updated_by)
values ('568fb1ec-d894-4202-957e-0caf3a506a59', 'RESUME_REASON_TYPE', now(), 'SYSTEM', now(), 'SYSTEM');

insert into mpozei.reason_nomenclature(id, create_date, created_by, last_update, updated_by, active, description, language, name, nomenclature_type)
values
('a380962f-ed2f-4c3a-a36d-f56892aed708', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Resumed by citizen', 'EN', 'RESUMED_BY_CITIZEN', '568fb1ec-d894-4202-957e-0caf3a506a59'),
('1dcbbcbb-d619-4cda-960a-e77b2ddde2f4', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Възобновено от гражданин', 'BG', 'RESUMED_BY_CITIZEN', '568fb1ec-d894-4202-957e-0caf3a506a59'),
('52e2a3db-bf5d-4dfd-9bbe-a9cc8a49c53b', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Resumed by employee/system', 'EN', 'RESUMED_BY_SYSTEM_OR_EMPLOYEE', '568fb1ec-d894-4202-957e-0caf3a506a59'),
('43ff8ce4-5a47-43a3-be2d-a00e67f7a5ae', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Възобновено от служител/система', 'BG', 'RESUMED_BY_SYSTEM_OR_EMPLOYEE', '568fb1ec-d894-4202-957e-0caf3a506a59'),
('cece89e2-5469-4d19-8288-186507ce45f9', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Other', 'EN', 'RESUMED_OTHER', '568fb1ec-d894-4202-957e-0caf3a506a59'),
('1c8996e6-596b-4a17-9fca-8f114c102c52', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Друго', 'BG', 'RESUMED_OTHER', '568fb1ec-d894-4202-957e-0caf3a506a59');
