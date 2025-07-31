ALTER TABLE  mpozei.reason_nomenclature
DROP COLUMN if exists internal;

ALTER TABLE mpozei.reason_nomenclature
ADD COLUMN  permitted_user varchar(255) NOT NULL default 'PRIVATE';

UPDATE  mpozei.reason_nomenclature
SET permitted_user = 'PUBLIC'
WHERE name in ('STOPPED_CANCELED_BY_USER');

UPDATE mpozei.reason_nomenclature 
SET permitted_user = 'ADMIN'
WHERE name in ('REVOKED_BY_ADMINISTRATOR', 'STOPPED_BY_ADMINISTRATOR', 'STOPPED_OTHER', 'DENIED_BY_ADMINISTRATOR', 'DENIED_OTHER', 'REVOKED_OTHER');

UPDATE mpozei.reason_nomenclature 
SET nomenclature_type = '5dfa852a-0094-43a6-be10-39e26aeb9232'
WHERE name in ('REVOKED_BY_NAIF');

UPDATE mpozei.reason_nomenclature 
SET nomenclature_type = '227438ca-19cc-4dce-8fe0-a2baeffb6f4e'
WHERE name in ('STOPPED_BY_NAIF');
