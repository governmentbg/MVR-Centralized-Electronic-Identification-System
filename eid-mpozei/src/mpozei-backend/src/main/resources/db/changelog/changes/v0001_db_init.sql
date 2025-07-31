create table eid_administrator
(
    id   uuid not null primary key,
    name varchar(255)
);

--alter table eid_administrator owner to postgres;

create table administrator_front_office
(
    id                   uuid not null primary key,
    name                 varchar(255),
    location             varchar(255),
    eid_administrator_id uuid
        constraint fkaa7ai8y75cdlk4eit2m97vujj
            references eid_administrator
);

--alter table administrator_front_office owner to postgres;

create table mpozei.nomenclature_type
(
    id          uuid not null
        primary key,
    name        varchar(255),
    create_date timestamp(6),
    created_by  varchar(255),
    last_update timestamp(6),
    updated_by  varchar(255)
);

--alter table mpozei.nomenclature_type owner to postgres;

create table mpozei.reason_nomenclature
(
    id                uuid not null
        primary key,
    create_date       timestamp(6),
    created_by        varchar(255),
    last_update       timestamp(6),
    updated_by        varchar(255),
    active            boolean default true,
    description       varchar(255),
    language          varchar(255),
    name              varchar(255),
    nomenclature_type uuid not null
        constraint fk4b116n7bw3q18i6h8amecauet
            references mpozei.nomenclature_type
);

--alter table mpozei.reason_nomenclature owner to postgres;

create table application
(
    id                        uuid        not null primary key,
    application_type          varchar(31) not null,
    application_number        varchar(255),
    status                    varchar(255),
    pipeline_status           varchar(255),
    eidentity_id              uuid,
    first_name                varchar(255),
    second_name               varchar(255),
    last_name                 varchar(255),
    citizen_identifier_number varchar(255),
    citizen_identifier_type   varchar(255),
    device_id                 uuid,
    signature                 oid,
    create_date               timestamp(6),
    created_by                uuid,
    last_update               timestamp(6),
    updated_by                uuid,
    params                    jsonb,
    eid_administrator_id      uuid
        constraint fk31fpa8k96fo5x0jofn9vokmn8
            references eid_administrator,
    reason_id                 uuid
        constraint fki01wwj9cano14h8mxotgh6lka
            references mpozei.reason_nomenclature,
    reason_text               varchar(255)
);

--alter table application owner to postgres;

create table mpozei.eid_issuer_nomenclature
(
    id                   uuid not null
        primary key,
    create_date          timestamp(6),
    created_by           varchar(255),
    last_update          timestamp(6),
    updated_by           varchar(255),
    active               boolean default true,
    description          varchar(255),
    language             varchar(255),
    name                 varchar(255),
    eid_administrator_id uuid not null,
    nomenclature_type    uuid not null
        constraint fk4cq0f3uvrg3s420vtclml732i
            references mpozei.nomenclature_type
);

--alter table mpozei.eid_issuer_nomenclature owner to postgres;

create sequence if not exists mpozei.revinfo_seq increment by 50;
--alter sequence if exists mpozei.revinfo_seq owner to postgres;