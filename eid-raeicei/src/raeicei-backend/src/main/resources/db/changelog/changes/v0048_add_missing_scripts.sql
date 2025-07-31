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


create sequence if not exists raeicei.revinfo_seq increment by 50;

alter table if exists raeicei.devices add column IF NOT EXISTS authorization_link varchar(255);
alter table if exists raeicei.devices add column IF NOT EXISTS backchannel_authorization_link varchar(255);
alter table if exists raeicei.devices_aud add column IF NOT EXISTS authorization_link varchar(255);
alter table if exists raeicei.devices_aud add column IF NOT EXISTS backchannel_authorization_link varchar(255);
alter table if exists raeicei.devices_aud add column IF NOT EXISTS is_active boolean;
alter table if exists raeicei.front_office add column IF NOT EXISTS description varchar(255);
alter table if exists raeicei.front_office_aud add column IF NOT EXISTS code varchar(4);
alter table if exists raeicei.front_office_aud add column IF NOT EXISTS description varchar(255);
alter table if exists raeicei.documents_aud alter column content set data type bytea;

create table IF NOT EXISTS raeicei.number_generator (type smallint not null check (type between 0 and 3), counter integer DEFAULT 0, primary key (type));

create table IF NOT EXISTS raeicei.number_generator_aud (type smallint not null check (type between 0 and 3), rev integer not null, revtype smallint, counter integer DEFAULT 0, primary key (rev, type));

alter table if exists raeicei.provided_service alter column service_type set data type varchar(255);
alter table if exists raeicei.provided_service_aud alter column service_type set data type varchar(255);

create table IF NOT EXISTS raeicei.application_attachments (application_id uuid not null, atachment_id uuid not null);
create table IF NOT EXISTS raeicei.application_authorized_persons (application_id uuid not null, authorized_person_id uuid not null);
create table IF NOT EXISTS raeicei.application_devices (application_id uuid not null, device_id uuid not null);
create table IF NOT EXISTS raeicei.application_emploees (application_id uuid not null, employee_id uuid not null);
create table IF NOT EXISTS raeicei.application_offices (application_id uuid not null, office_id uuid not null);

DROP TABLE IF EXISTS raeicei.document_descriptions_aud;
create table IF NOT EXISTS raeicei.document_descriptions_aud (rev integer not null, document_id uuid not null, description varchar(255) not null, language smallint not null check (language between 0 and 1), revtype smallint, primary key (document_id, rev, description, language));

create table IF NOT EXISTS raeicei.eid_manager_employees (manager_id uuid not null, employee_id uuid not null);

DROP TABLE IF EXISTS raeicei.eid_manager_offices_aud;
create table IF NOT EXISTS raeicei.eid_manager_offices_aud (office_id uuid not null, rev integer not null, eid_manager_id uuid, primary key (rev, office_id));

DROP TABLE IF EXISTS raeicei.employee_aud;
create table IF NOT EXISTS raeicei.employee_aud (id uuid not null, rev integer not null, revtype smallint, citizen_identifier_number varchar(10), citizen_identifier_type varchar(255) check (citizen_identifier_type in ('EGN','LNCh','FP')), email varchar(255), is_active boolean, name varchar(255), name_latin varchar(255), phone_number varchar(255), primary key (rev, id));

create table IF NOT EXISTS raeicei.employee_roles (employee_id uuid not null, roles varchar(255));

DROP TABLE IF EXISTS raeicei.employee_roles_aud;
create table IF NOT EXISTS raeicei.employee_roles_aud (rev integer not null, employee_id uuid not null, roles varchar(255) not null, revtype smallint, primary key (employee_id, rev, roles));

DROP TABLE IF EXISTS raeicei.jt_application_attachments_aud;
create table IF NOT EXISTS raeicei.jt_application_attachments_aud (rev integer not null, application_id uuid not null, atachment_id uuid not null, revtype smallint, primary key (application_id, rev, atachment_id));

DROP TABLE IF EXISTS raeicei.jt_application_authorized_persons_aud;
create table IF NOT EXISTS raeicei.jt_application_authorized_persons_aud (rev integer not null, application_id uuid not null, authorized_person_id uuid not null, revtype smallint, primary key (application_id, rev, authorized_person_id));

DROP TABLE IF EXISTS raeicei.jt_application_devices_aud;
create table IF NOT EXISTS raeicei.jt_application_devices_aud (rev integer not null, application_id uuid not null, device_id uuid not null, revtype smallint, primary key (application_id, rev, device_id));

DROP TABLE IF EXISTS raeicei.jt_application_emploees_aud;
create table IF NOT EXISTS raeicei.jt_application_emploees_aud (rev integer not null, application_id uuid not null, employee_id uuid not null, revtype smallint, primary key (application_id, rev, employee_id));

DROP TABLE IF EXISTS raeicei.jt_application_notes_aud;
create table IF NOT EXISTS raeicei.jt_application_notes_aud (rev integer not null, application_id uuid not null, note_id uuid not null, revtype smallint, primary key (application_id, rev, note_id));

DROP TABLE IF EXISTS raeicei.jt_application_offices_aud;
create table IF NOT EXISTS raeicei.jt_application_offices_aud (rev integer not null, application_id uuid not null, office_id uuid not null, revtype smallint, primary key (application_id, rev, office_id));

DROP TABLE IF EXISTS raeicei.jt_eid_manager_attachments_aud;
create table IF NOT EXISTS raeicei.jt_eid_manager_attachments_aud (rev integer not null, eid_manager_id uuid not null, atachment_id uuid not null, revtype smallint, primary key (eid_manager_id, rev, atachment_id));

DROP TABLE IF EXISTS raeicei.jt_eid_manager_authorized_persons_aud;
create table IF NOT EXISTS raeicei.jt_eid_manager_authorized_persons_aud (rev integer not null, eid_manager_id uuid not null, authorized_person_id uuid not null, revtype smallint, primary key (eid_manager_id, rev, authorized_person_id));

DROP TABLE IF EXISTS raeicei.jt_eid_manager_employees_aud;
create table IF NOT EXISTS raeicei.jt_eid_manager_employees_aud (rev integer not null, manager_id uuid not null, employee_id uuid not null, revtype smallint, primary key (manager_id, rev, employee_id));

DROP TABLE IF EXISTS raeicei.jt_eid_manager_notes_aud;
create table IF NOT EXISTS raeicei.jt_eid_manager_notes_aud (rev integer not null, eid_manager_id uuid not null, note_id uuid not null, revtype smallint, primary key (eid_manager_id, rev, note_id));

DROP TABLE IF EXISTS raeicei.jt_eid_manager_offices_aud;
create table IF NOT EXISTS raeicei.jt_eid_manager_offices_aud (rev integer not null, eid_manager_id uuid not null, office_id uuid not null, revtype smallint, primary key (eid_manager_id, rev, office_id));


SELECT drop_constraint('raeicei', 'application_attachments', 'atachment_id', 'u');
alter table if exists raeicei.application_attachments add constraint uk_application_atachment_id unique (atachment_id);

SELECT drop_constraint('raeicei', 'application_authorized_persons', 'application_number_id', 'u');
alter table if exists raeicei.application_authorized_persons add constraint uk_app_auth_persons_application_number_id unique (authorized_person_id);

SELECT drop_constraint('raeicei', 'application_devices', 'device_id', 'u');
alter table if exists raeicei.application_devices add constraint uk_application_devices_device_id unique (device_id);

SELECT drop_constraint('raeicei', 'application_emploees', 'employee_id', 'u');
alter table if exists raeicei.application_emploees add constraint uk_application_emploees_employee_id unique (employee_id);

SELECT drop_constraint('raeicei', 'eid_manager_employees', 'employee_id', 'u');
alter table if exists raeicei.eid_manager_employees add constraint uk_eid_manager_employees_employee_id unique (employee_id);

SELECT drop_constraint('raeicei', 'application_offices', 'office_id', 'u');
alter table if exists raeicei.application_offices add constraint uk_application_offices_office_id unique (office_id);

SELECT drop_constraint('raeicei', 'discounts', 'provided_service', 'f');
alter table if exists raeicei.discounts add constraint fk_provided_service_discounts foreign key (provided_service_id) references raeicei.provided_service;

SELECT drop_constraint('raeicei', 'document_types_aud', 'rev', 'f');
alter table if exists raeicei.document_types_aud add constraint fk_revinfo_document_types_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'documents', 'document_type', 'f');
alter table if exists raeicei.documents add constraint fk_document_types_documents foreign key (document_type) references raeicei.document_types;

SELECT drop_constraint('raeicei', 'documents_aud', 'rev', 'f');
alter table if exists raeicei.documents_aud add constraint fk_revinfo_documents_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'notes_aud', 'rev', 'f');
alter table if exists raeicei.notes_aud add constraint fk_revinfo_notes_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'number_generator_aud', 'rev', 'f');
alter table if exists raeicei.number_generator_aud add constraint fk_revinfo_number_generator_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'application_attachments', 'atachment_id', 'f');
alter table if exists raeicei.application_attachments add constraint fk_documents_application_attachments foreign key (atachment_id) references raeicei.documents;

SELECT drop_constraint('raeicei', 'application_attachments', 'application_id', 'f');
alter table if exists raeicei.application_attachments add constraint fk_application_application_attachments foreign key (application_id) references raeicei.application;

SELECT drop_constraint('raeicei', 'application_authorized_persons', 'authorized_person_id', 'f');
alter table if exists raeicei.application_authorized_persons add constraint fk_contact_application_authorized_persons foreign key (authorized_person_id) references raeicei.contact;

SELECT drop_constraint('raeicei', 'application_authorized_persons', 'application_id', 'f');
alter table if exists raeicei.application_authorized_persons add constraint fk_application_application_authorized_persons foreign key (application_id) references raeicei.application;

SELECT drop_constraint('raeicei', 'application_devices', 'device_id', 'f');
alter table if exists raeicei.application_devices add constraint fk_devices_application_devices foreign key (device_id) references raeicei.devices;

SELECT drop_constraint('raeicei', 'application_devices', 'application_id', 'f');
alter table if exists raeicei.application_devices add constraint fk_application_application_devices foreign key (application_id) references raeicei.application;

SELECT drop_constraint('raeicei', 'application_emploees', 'employee_id', 'f');
alter table if exists raeicei.application_emploees add constraint fk_employee_application_emploees foreign key (employee_id) references raeicei.employee;

SELECT drop_constraint('raeicei', 'application_emploees', 'application_id', 'f');
alter table if exists raeicei.application_emploees add constraint fk_application_application_emploees foreign key (application_id) references raeicei.application;    
 
SELECT drop_constraint('raeicei', 'application_offices', 'office_id', 'f');
alter table if exists raeicei.application_offices add constraint fk_front_office_application_offices foreign key (office_id) references raeicei.front_office;

SELECT drop_constraint('raeicei', 'application_offices', 'application_id', 'f');
alter table if exists raeicei.application_offices add constraint fk_application_application_offices foreign key (application_id) references raeicei.application;

SELECT drop_constraint('raeicei', 'document_descriptions_aud', 'rev', 'f');
alter table if exists raeicei.document_descriptions_aud add constraint fk_revinfo_document_descriptions_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'eid_manager_employees', 'employee_id', 'f');
alter table if exists raeicei.eid_manager_employees add constraint fk_employee_eid_manager_employees foreign key (employee_id) references raeicei.employee;

SELECT drop_constraint('raeicei', 'eid_manager_employees', 'manager_id', 'f');
alter table if exists raeicei.eid_manager_employees add constraint fk_eid_manager_eid_manager_employees foreign key (manager_id) references raeicei.eid_manager;

SELECT drop_constraint('raeicei', 'eid_manager_offices_aud', 'rev', 'f');
alter table if exists raeicei.eid_manager_offices_aud add constraint fk_front_office_aud_eid_manager_offices_aud foreign key (rev, office_id) references raeicei.front_office_aud;

SELECT drop_constraint('raeicei', 'employee_aud', 'rev', 'f');
alter table if exists raeicei.employee_aud add constraint fk_revinfo_employee_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'employee_roles', 'employee_id', 'f');
alter table if exists raeicei.employee_roles add constraint fk_employee_employee_roles foreign key (employee_id) references raeicei.employee;

SELECT drop_constraint('raeicei', 'employee_roles_aud', 'rev', 'f');
alter table if exists raeicei.employee_roles_aud add constraint fk_revinfo_employee_roles_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'jt_application_attachments_aud', 'rev', 'f');
alter table if exists raeicei.jt_application_attachments_aud add constraint fk_revinfo_jt_application_attachments_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'jt_application_authorized_persons_aud', 'rev', 'f');
alter table if exists raeicei.jt_application_authorized_persons_aud add constraint fk_revinfo_jt_application_authorized_persons_aud foreign key (rev) references raeicei.revinfo;   

SELECT drop_constraint('raeicei', 'jt_application_devices_aud', 'rev', 'f');
alter table if exists raeicei.jt_application_devices_aud add constraint fk_revinfo_jt_application_devices_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'jt_application_emploees_aud', 'rev', 'f');
alter table if exists raeicei.jt_application_emploees_aud add constraint fk_revinfo_jt_application_emploees_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'jt_application_notes_aud', 'rev', 'f');
alter table if exists raeicei.jt_application_notes_aud add constraint fk_revinfo_jt_application_notes_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'jt_application_offices_aud', 'rev', 'f');
alter table if exists raeicei.jt_application_offices_aud add constraint fk_revinfo_jt_application_offices_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'jt_eid_manager_attachments_aud', 'rev', 'f');
alter table if exists raeicei.jt_eid_manager_attachments_aud add constraint fk_revinfo_jt_eid_manager_attachments_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'jt_eid_manager_authorized_persons_aud', 'rev', 'f');
alter table if exists raeicei.jt_eid_manager_authorized_persons_aud add constraint fk_revinfo_jt_eid_manager_authorized_persons_aud foreign key (rev) references raeicei.revinfo;   
 
SELECT drop_constraint('raeicei', 'jt_eid_manager_employees_aud', 'rev', 'f');
alter table if exists raeicei.jt_eid_manager_employees_aud add constraint fk_revinfo_jt_eid_manager_employees_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'jt_eid_manager_notes_aud', 'rev', 'f');
alter table if exists raeicei.jt_eid_manager_notes_aud add constraint fk_revinfo_jt_eid_manager_notes_aud foreign key (rev) references raeicei.revinfo;

SELECT drop_constraint('raeicei', 'jt_eid_manager_offices_aud', 'rev', 'f');
alter table if exists raeicei.jt_eid_manager_offices_aud add constraint fk_revinfo_jt_eid_manager_offices_aud foreign key (rev) references raeicei.revinfo;