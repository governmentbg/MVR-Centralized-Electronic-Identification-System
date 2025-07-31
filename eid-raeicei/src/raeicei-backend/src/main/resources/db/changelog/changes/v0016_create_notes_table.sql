CREATE TABLE IF NOT EXISTS
    notes
(
    id               UUID                   NOT NULL,
    create_date      TIMESTAMP(6) WITHOUT TIME ZONE,
    created_by       CHARACTER VARYING(255),
    last_update      TIMESTAMP(6) WITHOUT TIME ZONE,
    updated_by       CHARACTER VARYING(255),
    VERSION          BIGINT,
    authors_username CHARACTER VARYING(255) NOT NULL,
    CONTENT          CHARACTER VARYING(255) NOT NULL,
    is_outgoing      BOOLEAN                NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE
    application_notes
(
    application_id UUID NOT NULL,
    note_id        UUID NOT NULL,
    CONSTRAINT fkk5ipew5saakavrkfdpd288qm5 FOREIGN KEY (note_id) REFERENCES
        "notes" ("id"),
    CONSTRAINT fkaxkfhyvof8ymgbo1is28jpovr FOREIGN KEY (application_id) REFERENCES
        "application" ("id"),
    CONSTRAINT uka1963b4tjowpobi561sk597wv UNIQUE (note_id)
);