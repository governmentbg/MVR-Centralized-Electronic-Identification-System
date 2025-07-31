CREATE TABLE IF NOT EXISTS
    documents
(
    id UUID NOT NULL,
    create_date TIMESTAMP(6) WITHOUT TIME ZONE,
    created_by  CHARACTER VARYING(255),
    last_update TIMESTAMP(6) WITHOUT TIME ZONE,
    updated_by  CHARACTER VARYING(255),
    VERSION     BIGINT,
    CONTENT OID NOT NULL,
    file_name CHARACTER VARYING(255) NOT NULL,
    file_path CHARACTER VARYING(255) NOT NULL,
    document_type UUID,
    is_outgoing BOOLEAN NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS
    documents_aud
(
    id UUID NOT NULL,
    rev     INTEGER NOT NULL,
    revtype SMALLINT,
    CONTENT BYTEA,
    file_name CHARACTER VARYING(255),
    file_path CHARACTER VARYING(255),
    document_type UUID,
    is_outgoing BOOLEAN NOT NULL,
    PRIMARY KEY (rev, id)
);

CREATE TABLE IF NOT EXISTS
    notes_aud
(
    id UUID NOT NULL,
    rev              INTEGER NOT NULL,
    revtype          SMALLINT,
    authors_username CHARACTER VARYING(255),
    CONTENT          CHARACTER VARYING(255),
    is_outgoing      BOOLEAN,
    new_status       CHARACTER VARYING(255) NOT NULL,
    PRIMARY KEY (rev, id)
);

ALTER TABLE raeicei.document_types
    ADD COLUMN IF NOT EXISTS required_for_administrator BOOLEAN;

ALTER TABLE raeicei.document_types
    ADD COLUMN IF NOT EXISTS required_for_center BOOLEAN;

CREATE TABLE IF NOT EXISTS
    document_types_aud
(
    id UUID NOT NULL,
    rev                        INTEGER NOT NULL,
    revtype                    SMALLINT,
    active                     BOOLEAN,
    NAME                       CHARACTER VARYING(255),
    required_for_center        BOOLEAN,
    required_for_administrator BOOLEAN,
    PRIMARY KEY (rev, id)
);

ALTER TABLE raeicei.eid_manager_aud
    ADD COLUMN IF NOT EXISTS code CHARACTER VARYING(3);