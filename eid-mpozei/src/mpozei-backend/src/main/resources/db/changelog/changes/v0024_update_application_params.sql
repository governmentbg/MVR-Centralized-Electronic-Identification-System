-- Create columns of in application
ALTER TABLE mpozei.application ADD COLUMN submission_type varchar(255);

ALTER TABLE mpozei.application ADD COLUMN administrator_front_office_id uuid;

-- Move data from fields in JSON column to separate columns
UPDATE  mpozei.application
SET submission_type = params->>'applicationSubmissionType';

UPDATE mpozei.application
SET administrator_front_office_id = (params->>'administratorFrontOfficeId')::uuid;

--Remove the JSON Field
UPDATE  mpozei.application
SET params = params-'applicationSubmissionType';

UPDATE mpozei.application
SET params = params-'administratorFrontOfficeId';