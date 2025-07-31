CREATE TABLE IF NOT EXISTS mpozei.application_number (id CHARACTER VARYING(255) NOT NULL, PRIMARY KEY (id));


ALTER TABLE mpozei.application_number ADD COLUMN administrator_code varchar(4);

ALTER TABLE mpozei.application_number ADD COLUMN office_code varchar(5);

UPDATE  mpozei.application_number
SET office_code = 'РПУ3', administrator_code = 'МВР';

ALTER TABLE 
   mpozei.application_number ALTER COLUMN administrator_code SET NOT NULL;

ALTER TABLE 
   mpozei.application_number ALTER COLUMN office_code SET NOT NULL;

drop table IF EXISTS mpozei.application_number_aud;
drop table IF EXISTS mpoze.number_generator_aud;