CREATE TABLE IF NOT EXISTS
    eid_manager_notes
(
    eid_manager_id UUID NOT NULL,
    note_id UUID NOT NULL,
    CONSTRAINT fksyabdofn6en53acje1nqf7t9n FOREIGN KEY (note_id) REFERENCES "notes" ("id"),
    CONSTRAINT fk9il2iuokpsch301db0pecbtju FOREIGN KEY (eid_manager_id) REFERENCES
        "eid_manager" ("id"),
    CONSTRAINT uk63v10xedjl85gscd5hdc140e7 UNIQUE (note_id)
);

CREATE TABLE IF NOT EXISTS
    eid_manager_attachments
(
    eid_manager_id UUID NOT NULL,
    atachment_id UUID NOT NULL,
    CONSTRAINT fkoed10vgfp3yt21mlw85g9nq0r FOREIGN KEY (atachment_id) REFERENCES "documents"
        ("id"),
    CONSTRAINT fksexfpwo72uve0yvygptf283ul FOREIGN KEY (eid_manager_id) REFERENCES
        "eid_manager" ("id"),
    CONSTRAINT ukq0dthp2oor4so70vyym8ii991 UNIQUE (atachment_id)
);