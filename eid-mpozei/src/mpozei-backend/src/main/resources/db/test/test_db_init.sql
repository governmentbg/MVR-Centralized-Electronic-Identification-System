CREATE SCHEMA IF NOT EXISTS mpozei;
SET SCHEMA mpozei;

create table if not exists eid_administrator
(
    id   uuid not null primary key,
    name varchar(255)
);

create table if not exists administrator_front_office
(
    id                   uuid not null primary key,
    name                 varchar(255),
    location             varchar(255),
    eid_administrator_id uuid,
    create_date               timestamp(6),
    created_by                uuid,
    last_update               timestamp(6),
    updated_by                uuid
);
ALTER TABLE administrator_front_office
    ADD FOREIGN KEY (eid_administrator_id)
        REFERENCES eid_administrator(id);

create table if not exists application
(
    id                        uuid        not null primary key,
    application_type          varchar(31) not null,
    application_number        varchar(255),
    application_status        varchar(255),
    pipeline_status           varchar(255),
    eidentity_id              uuid,
    first_name                varchar(255),
    second_name               varchar(255),
    last_name                 varchar(255),
    citizen_identifier_number varchar(255),
    citizen_identifier_type   varchar(255),
    device_id                 uuid,
    signature                 blob,
    create_date               timestamp(6),
    created_by                uuid,
    last_update               timestamp(6),
    updated_by                uuid,
    eid_administrator_id      uuid
        constraint fk31fpa8k96fo5x0jofn9vokmn8
            references eid_administrator
);

INSERT INTO mpozei.eid_administrator (id, name)
SELECT '194a90a0-3b9d-47f5-865a-ad8bcf2c3acc', 'MVR'
WHERE NOT EXISTS (SELECT '194a90a0-3b9d-47f5-865a-ad8bcf2c3acc'
                  FROM mpozei.eid_administrator
                  WHERE name = 'MVR');

CREATE ALIAS IF NOT EXISTS PGP_SYM_DECRYPT AS $$
@CODE
String PGP_SYM_DECRYPT(byte[] inputBytes, String password) {
    return new String(inputBytes);
}
$$;

CREATE ALIAS IF NOT EXISTS PGP_SYM_ENCRYPT AS $$
@CODE
byte[] PGP_SYM_ENCRYPT(String input, String password) {
    return input.getBytes();
}
$$;

CREATE TABLE IF NOT EXISTS nomenclature_type
(
    id  varchar(36)        primary key,
    name character varying(255),
    create_date timestamp(6) without time zone,
    created_by character varying(255),
    last_update timestamp(6) without time zone,
    updated_by character varying(255)
);


CREATE TABLE IF NOT EXISTS reason_nomenclature
(
    id uuid not null primary key,
    create_date timestamp(6) without time zone,
    created_by character varying(255),
    last_update timestamp(6) without time zone,
    updated_by character varying(255),
    active boolean DEFAULT true,
    description character varying(255),
    language character varying(255),
    name character varying(255),
    nomenclature_type uuid NOT NULL,
    text_required boolean DEFAULT false,
    internal boolean default false
);

create table if not exists number_generator
(
    id uuid not null primary key,
    counter integer
);

create table if not exists application_number
(
    id varchar(255) not null primary key
);

merge into mpozei.number_generator (id, counter) values ('eae8db59-b348-43e5-b21f-c9c9d0d65d9b', 0);

merge into mpozei.nomenclature_type (id, name, create_date, created_by, last_update, updated_by)
values ('227438ca-19cc-4dce-8fe0-a2baeffb6f4e', 'STOP_REASON_TYPE', now(), 'SYSTEM', now(), 'SYSTEM');

merge into mpozei.nomenclature_type (id, name, create_date, created_by, last_update, updated_by)
values ('c655a8c3-19ea-4dc2-97b7-b4b4c28df195', 'DENIED_REASON_TYPE', now(), 'SYSTEM', now(), 'SYSTEM');

merge into mpozei.reason_nomenclature(id, create_date, created_by, last_update, updated_by, active, description, language, name, nomenclature_type, text_required)
values
('5cc4daef-835c-4d0c-a842-a7cfe4c8cc87', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Canceled by user', 'EN', 'STOPPED_CANCELED_BY_USER', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e', false),
('12729da8-e2b9-475f-b15f-86e8b51ea78a', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Отказано от гражданин', 'BG', 'STOPPED_CANCELED_BY_USER', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e', false),
('2961b028-d074-423c-b1f0-9fb236aa9a59', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Removed by system', 'EN', 'STOPPED_REVOKED_BY_SYSTEM', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e', false),
('93f874be-577e-44f1-a3fb-3f53b19e90db', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Премахнато от системата', 'BG', 'STOPPED_REVOKED_BY_SYSTEM', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e', false),
('5ae29a8d-3611-4778-8b14-a0baa360e53b', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Stopped by administrator', 'EN', 'STOPPED_BY_ADMINISTRATOR', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e', false),
('d21d2c75-c31e-4075-a92a-77b8c4a05a6e', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Спряно от администратор', 'BG', 'STOPPED_BY_ADMINISTRATOR', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e', false),
('6cd1968f-b27e-4ba3-a5d9-541e46ade713', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Other', 'EN', 'STOPPED_OTHER', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e', false),
('51021190-64de-4714-a4e3-82180e4d2750', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Друго', 'BG', 'STOPPED_OTHER', '227438ca-19cc-4dce-8fe0-a2baeffb6f4e', false);


merge into mpozei.reason_nomenclature(id, create_date, created_by, last_update, updated_by, active, description, language, name, nomenclature_type)
values
('36325da5-54a4-42e8-9bd8-b04e69c65bf0', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Denied by administrator', 'EN', 'DENIED_BY_ADMINISTRATOR', 'c655a8c3-19ea-4dc2-97b7-b4b4c28df195'),
('2e1db2b2-0318-4433-8a19-e78e6050b1a8', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Отказано от администратор', 'BG', 'DENIED_BY_ADMINISTRATOR', 'c655a8c3-19ea-4dc2-97b7-b4b4c28df195'),
('82f75af0-c6db-49f8-9ff5-005c961f60f0', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Other', 'EN', 'DENIED_OTHER', 'c655a8c3-19ea-4dc2-97b7-b4b4c28df195'),
('a7216f16-24c2-4088-95fa-a2b0aeca155f', now(), 'SYSTEM', now(), 'SYSTEM', true, 'Друго', 'BG', 'DENIED_OTHER', 'c655a8c3-19ea-4dc2-97b7-b4b4c28df195');

merge into administrator_front_office (id, name, location, eid_administrator_id, create_date, created_by, last_update, updated_by)
values ('af7c1fe6-d669-414e-b066-e9733f0de7a8', 'MVR', 'Sofia', '194a90a0-3b9d-47f5-865a-ad8bcf2c3acc', now(), null, now(), null);

