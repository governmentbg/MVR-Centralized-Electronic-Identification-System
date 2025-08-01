insert into mpozei.nomenclature_type (id, name, create_date, created_by, last_update, updated_by)
values ('227438ca-19cc-4dce-8fe0-a2baeffb6f4e', 'STOP_REASON_TYPE', now(), 'SYSTEM', now(), 'SYSTEM');

insert into mpozei.reason_nomenclature(id, create_date, created_by, last_update, updated_by, active, description, language, name, nomenclature_type)
values
('5cc4daef-835c-4d0c-a842-a7cfe4c8cc87', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Canceled by user', 'EN', 'STOPPED_CANCELED_BY_USER', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e'),
('12729da8-e2b9-475f-b15f-86e8b51ea78a', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Отказано от гражданин', 'BG', 'STOPPED_CANCELED_BY_USER', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e'),
('2961b028-d074-423c-b1f0-9fb236aa9a59', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Removed by system', 'EN', 'STOPPED_REVOKED_BY_SYSTEM', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e'),
('93f874be-577e-44f1-a3fb-3f53b19e90db', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Премахнато от системата', 'BG', 'STOPPED_REVOKED_BY_SYSTEM', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e'),
('6cd1968f-b27e-4ba3-a5d9-541e46ade713', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Other', 'EN', 'STOPPED_OTHER', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e'),
('51021190-64de-4714-a4e3-82180e4d2750', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Друго', 'BG', 'STOPPED_OTHER', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e');
