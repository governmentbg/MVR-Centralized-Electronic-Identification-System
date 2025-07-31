CREATE OR REPLACE FUNCTION drop_constraint(schema_name TEXT, table_name TEXT, column_name TEXT, constraint_type CHAR)
RETURNS void AS
$$
DECLARE
    constraint_name TEXT;
BEGIN
    SELECT con.conname
    INTO constraint_name
    FROM pg_constraint con
    JOIN pg_class tbl ON tbl.oid = con.conrelid
    JOIN pg_namespace ns ON ns.oid = tbl.relnamespace
    JOIN pg_attribute att ON att.attrelid = tbl.oid AND att.attnum = ANY(con.conkey)
    WHERE con.contype = constraint_type
      AND ns.nspname = schema_name
      AND tbl.relname = table_name
      AND att.attname = column_name
    LIMIT 1;

    IF constraint_name IS NOT NULL THEN
        EXECUTE format(
            'ALTER TABLE %I.%I DROP CONSTRAINT %I;',
            schema_name, table_name, constraint_name
        );
    END IF;
END;
$$ LANGUAGE plpgsql;

ALTER TABLE mpozei.reason_nomenclature
ADD COLUMN IF NOT EXISTS version bigint;

ALTER TABLE mpozei.nomenclature_type
ADD COLUMN IF NOT EXISTS version bigint;

ALTER TABLE mpozei.application
DROP COLUMN IF EXISTS application_number;

alter table if exists mpozei.application alter column created_by set data type varchar(255);
alter table if exists mpozei.application alter column updated_by set data type varchar(255);
alter table if exists mpozei.application add column IF NOT EXISTS version bigint;
alter table if exists mpozei.application add column IF NOT EXISTS  application_xml text;
alter table if exists mpozei.application add column IF NOT EXISTS  citizen_profile_id uuid;
alter table if exists mpozei.application add column IF NOT EXISTS  citizenship varchar(255);
alter table if exists mpozei.application add column IF NOT EXISTS  detached_signature text;
alter table if exists mpozei.application add column IF NOT EXISTS  first_name_latin varchar(255);
alter table if exists mpozei.application add column IF NOT EXISTS  last_name_latin varchar(255);
alter table if exists mpozei.application add column IF NOT EXISTS  second_name_latin varchar(255);
alter table if exists mpozei.application add column IF NOT EXISTS  application_number_id varchar(255);

DROP TABLE IF EXISTS mpozei.application_aud;
create table IF NOT EXISTS mpozei.application_aud (id uuid not null, rev integer not null, application_type varchar(31) not null, revtype smallint, administrator_front_office_id uuid, application_xml text, citizen_identifier_number bytea, citizen_identifier_type varchar(255) check (citizen_identifier_type in ('EGN','LNCh','FP')), citizen_profile_id uuid, citizenship varchar(255), detached_signature text, device_id uuid, eid_administrator_id uuid, eidentity_id uuid, first_name varchar(255), first_name_latin varchar(255), last_name varchar(255), last_name_latin varchar(255), params jsonb, pipeline_status varchar(255) check (pipeline_status in ('INITIATED','EJBCA_CERTIFICATE_CREATION','REGIX_VERIFICATION','REI_EIDENTITY_CREATION','REI_EIDENTITY_VERIFICATION','RUEI_BASE_PROFILE_ATTACHMENT','RUEI_BASE_PROFILE_VERIFICATION','RUEI_CERTIFICATE_CREATION','SIGNATURE_VERIFICATION','SIGNATURE_CREATION','RUEI_CERTIFICATE_STATUS_VERIFICATION''RUEI_CERTIFICATE_RETRIEVAL','EJBCA_CERTIFICATE_REVOCATION','RUEI_CHANGE_EID_STATUS','EJBCA_CERTIFICATE_RETRIEVAL','EJBCA_END_ENTITY_CREATION','PUN_CERTIFICATE_CREATION','PUN_CARRIER_RETRIEVAL','ISSUE_EID_SIGNED','SEND_NOTIFICATION','CERTIFICATE_HISTORY_CREATION','PIVR_VERIFICATION','EXPORT_APPLICATION','DENIED_APPLICATION','E_DELIVERY_NOTIFICATION','XML_TIMESTAMP','CREATE_PAYMENT','CALCULATE_PAYMENT','EXISTING_CERTIFICATE_VERIFICATION','EXISTING_APPLICATION_VERIFICATION','EXT_ADMIN_CERTIFICATE_RETRIEVAL','EXT_ADMIN_CERTIFICATE_CREATION')), reason_text varchar(255), second_name varchar(255), second_name_latin varchar(255), status varchar(255) check (status in ('SUBMITTED','PROCESSING','PENDING_SIGNATURE','SIGNED','PENDING_PAYMENT','PAYMENT_EXPIRED','PAID','DENIED','APPROVED','GENERATED_CERTIFICATE','COMPLETED','CERTIFICATE_STORED')), submission_type varchar(255) check (submission_type in ('DESK','BASE_PROFILE','EID','PERSO_CENTRE')), reason_id uuid, primary key (rev, id));

alter table if exists mpozei.application_number alter column administrator_code set data type varchar(255);
alter table if exists mpozei.application_number alter column office_code set data type varchar(255);

DROP TABLE IF EXISTS mpozei.application_number_aud;
create table IF NOT EXISTS mpozei.application_number_aud (id varchar(255) not null, rev integer not null, revtype smallint, administrator_code varchar(255), office_code varchar(255), primary key (rev, id));

DROP TABLE IF EXISTS mpozei.number_generator_aud;
create table IF NOT EXISTS mpozei.number_generator_aud (administrator_code varchar(4) not null, office_code varchar(5) not null, rev integer not null, revtype smallint, counter integer DEFAULT 0, primary key (rev, administrator_code, office_code));

DROP TABLE IF EXISTS mpozei.reason_nomenclature_aud;
create table IF NOT EXISTS mpozei.reason_nomenclature_aud (id uuid not null, rev integer not null, revtype smallint, active boolean default true, description varchar(255), language varchar(255) check (language in ('EN','BG')), name varchar(255), nomenclature_type uuid, permitted_user varchar(255) check (permitted_user in ('PUBLIC','ADMIN','PRIVATE')), text_required boolean, primary key (rev, id));

DROP TABLE IF EXISTS mpozei.certificate_history_aud;
create table IF NOT EXISTS mpozei.certificate_history_aud (id uuid not null, rev integer not null, revtype smallint, application_id uuid, application_number varchar(255), certificate_id uuid, create_date timestamp(6) with time zone, device_id uuid, modified_date timestamp(6) with time zone, reason_id uuid, reason_text varchar(255), status varchar(255) check (status in ('CREATED','SIGNED','ACTIVE','STOPPED','REVOKED','INVALID','FAILED','EXPIRED')), validity_from timestamp(6) with time zone, validity_until timestamp(6) with time zone, primary key 
(rev, id));

create table IF NOT EXISTS mpozei.certificate_history (id uuid not null, application_id uuid, application_number varchar(255), certificate_id uuid, create_date timestamp(6) with time zone, device_id uuid, modified_date timestamp(6) with time zone, reason_id uuid, reason_text varchar(255), status varchar(255) check (status in ('CREATED','SIGNED','ACTIVE','STOPPED','REVOKED','INVALID','FAILED','EXPIRED')), validity_from timestamp(6) with time zone, validity_until timestamp(6) with time zone, primary key (id));

DROP TABLE IF EXISTS mpozei.nomenclature_type_aud;
create table IF NOT EXISTS mpozei.nomenclature_type_aud (id uuid not null, rev integer not null, revtype smallint, name varchar(255), primary key (rev, id));

create table IF NOT EXISTS mpozei.revinfo (rev integer not null, revtstmp bigint, primary key (rev));

SELECT drop_constraint('mpozei', 'application', 'application_number_id', 'u');
alter table if exists mpozei.application add constraint uk_application_application_number_id unique (application_number_id);

SELECT drop_constraint('mpozei', 'application', 'application_number_id', 'f');
alter table if exists mpozei.application add constraint fk_application_number_application foreign key (application_number_id) references mpozei.application_number;

SELECT drop_constraint('mpozei', 'application_aud', 'rev', 'f');
alter table if exists mpozei.application_aud add constraint fk_revinfo_application_aud foreign key (rev) references mpozei.revinfo;

SELECT drop_constraint('mpozei', 'application_number_aud', 'rev', 'f');
alter table if exists mpozei.application_number_aud add constraint fk_revinfo_application_number_aud foreign key (rev) references mpozei.revinfo;

SELECT drop_constraint('mpozei', 'number_generator_aud', 'rev', 'f');
alter table if exists mpozei.number_generator_aud add constraint fk_revinfo_number_generator_aud foreign key (rev) references mpozei.revinfo;

SELECT drop_constraint('mpozei', 'reason_nomenclature_aud', 'rev', 'f');
alter table if exists mpozei.reason_nomenclature_aud add constraint fk_revinfo_reason_nomenclature_aud foreign key (rev) references mpozei.revinfo;

SELECT drop_constraint('mpozei', 'certificate_history_aud', 'rev', 'f');
alter table if exists mpozei.certificate_history_aud add constraint fk_revinfo_certificate_history_aud foreign key (rev) references mpozei.revinfo;

SELECT drop_constraint('mpozei', 'nomenclature_type_aud', 'rev', 'f');
alter table if exists mpozei.nomenclature_type_aud add constraint fk_revinfo_nomenclature_type_aud foreign key (rev) references mpozei.revinfo;
