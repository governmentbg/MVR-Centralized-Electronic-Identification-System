DROP TABLE IF EXISTS mpozei.eid_issuer_nomenclature;

SET timezone = 'UTC';  -- make sure the time zone is set properly

ALTER TABLE mpozei.application ALTER create_date TYPE timestamptz;

ALTER TABLE mpozei.application ALTER last_update TYPE timestamptz;

ALTER TABLE mpozei.nomenclature_type ALTER create_date TYPE timestamptz;

ALTER TABLE mpozei.nomenclature_type ALTER last_update TYPE timestamptz;

ALTER TABLE mpozei.reason_nomenclature ALTER create_date  TYPE timestamptz;

ALTER TABLE mpozei.reason_nomenclature ALTER last_update TYPE timestamptz;