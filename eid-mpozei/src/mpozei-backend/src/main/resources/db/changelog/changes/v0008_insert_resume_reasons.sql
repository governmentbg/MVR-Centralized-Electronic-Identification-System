insert into mpozei.nomenclature_type (id, name, create_date, created_by, last_update, updated_by)
values ('c655a8c3-19ea-4dc2-97b7-b4b4c28df195', 'DENIED_REASON_TYPE', now(), 'SYSTEM', now(), 'SYSTEM');

insert into mpozei.reason_nomenclature(id, create_date, created_by, last_update, updated_by, active, description, language, name, nomenclature_type)
values
('36325da5-54a4-42e8-9bd8-b04e69c65bf0', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Denied by administrator', 'EN', 'DENIED_BY_ADMINISTRATOR', 'c655a8c3-19ea-4dc2-97b7-b4b4c28df195'),
('2e1db2b2-0318-4433-8a19-e78e6050b1a8', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Отказано от администратор', 'BG', 'DENIED_BY_ADMINISTRATOR', 'c655a8c3-19ea-4dc2-97b7-b4b4c28df195'),
('82f75af0-c6db-49f8-9ff5-005c961f60f0', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Other', 'EN', 'DENIED_OTHER', 'c655a8c3-19ea-4dc2-97b7-b4b4c28df195'),
('a7216f16-24c2-4088-95fa-a2b0aeca155f', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Друго', 'BG', 'DENIED_OTHER', 'c655a8c3-19ea-4dc2-97b7-b4b4c28df195');
