ALTER TABLE raeicei.document_types
DROP COLUMN IF EXISTS required;

ALTER TABLE raeicei.document_types_aud
DROP COLUMN IF EXISTS required;

ALTER TABLE raeicei.document_types
    ADD COLUMN IF NOT EXISTS required_for_administrator BOOLEAN;

ALTER TABLE raeicei.document_types_aud
    ADD COLUMN IF NOT EXISTS required_for_center BOOLEAN;