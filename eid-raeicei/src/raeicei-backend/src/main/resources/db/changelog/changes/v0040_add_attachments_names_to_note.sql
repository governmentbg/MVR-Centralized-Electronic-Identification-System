CREATE TABLE IF NOT EXISTS
    note_atachments_names
(
    note_id UUID NOT NULL,
    attachment_name CHARACTER VARYING(255),
    CONSTRAINT fkpq6099a6n2ibxhia345rp78dv FOREIGN KEY (note_id) REFERENCES "notes" ("id")
);

CREATE TABLE IF NOT EXISTS
    note_atachments_names_aud
(
    rev INTEGER NOT NULL,
    note_id UUID NOT NULL,
    attachment_name CHARACTER VARYING(255) NOT NULL,
    revtype         SMALLINT,
    PRIMARY KEY (note_id, rev, attachment_name),
    CONSTRAINT fkpp3dag18majeav7u0t29i1jx7 FOREIGN KEY (rev) REFERENCES "revinfo" ("rev")
);