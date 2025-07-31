CREATE TABLE IF NOT EXISTS document_types ( 
        id UUID NOT NULL, 
        active   BOOLEAN, 
        NAME     CHARACTER VARYING(255) NOT NULL, 
        required BOOLEAN, 
        PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS document_descriptions ( 
        document_id UUID NOT NULL, 
        description CHARACTER VARYING(255), 
        language    SMALLINT NOT NULL, 
        PRIMARY KEY (document_id, language), 
        CONSTRAINT fkpj5jldbaxw8xclyfgdrv256n4 FOREIGN KEY (document_id) REFERENCES 
        "document_types" ("id"), 
        CHECK ((language >= 0) 
    AND 
        ( 
            language <= 1) 
        ) 
);

INSERT INTO raeicei.document_types (id, active, name, required)
VALUES ('6fd7c15e-8cb1-45ed-8a84-7eb0503a4f2e', true, 'Test document type', true) ON CONFLICT (id) DO NOTHING;

INSERT INTO raeicei.document_descriptions (document_id, description, language)
VALUES ('6fd7c15e-8cb1-45ed-8a84-7eb0503a4f2e', 'Test description', 0) ON CONFLICT (document_id, language) DO NOTHING;