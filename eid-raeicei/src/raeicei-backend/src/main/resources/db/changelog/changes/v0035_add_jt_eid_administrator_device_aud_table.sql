DROP TABLE IF EXISTS
    eid_administrator_device_aud;

CREATE TABLE IF NOT EXISTS
    jt_eid_administrator_device_aud
(
    rev INTEGER NOT NULL,
    eid_administrator_id UUID NOT NULL,
    device_id UUID NOT NULL,
    revtype SMALLINT,
    PRIMARY KEY (eid_administrator_id, rev, device_id),
    CONSTRAINT fksjw7ymyt6towmr9wcju561yq6 FOREIGN KEY (rev) REFERENCES "revinfo" ("rev")
);