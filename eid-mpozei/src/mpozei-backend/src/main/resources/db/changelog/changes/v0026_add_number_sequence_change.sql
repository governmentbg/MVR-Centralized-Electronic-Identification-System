ALTER TABLE mpozei.number_generator ADD COLUMN administrator_code varchar(4);

ALTER TABLE mpozei.number_generator ADD COLUMN office_code varchar(5);

UPDATE  mpozei.number_generator
SET office_code = 'РПУ3', administrator_code = 'МВР'
WHERE id = 'eae8db59-b348-43e5-b21f-c9c9d0d65d9b';

ALTER TABLE 
   mpozei.number_generator ALTER COLUMN administrator_code SET NOT NULL;

ALTER TABLE 
   mpozei.number_generator ALTER COLUMN office_code SET NOT NULL;
   
ALTER TABLE 
   mpozei.number_generator DROP COLUMN id;
   
ALTER TABLE 
   mpozei.number_generator ADD PRIMARY KEY (administrator_code, office_code);   