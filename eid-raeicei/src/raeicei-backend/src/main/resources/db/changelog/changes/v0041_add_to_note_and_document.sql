ALTER TABLE raeicei.documents
    ADD COLUMN IF NOT EXISTS is_outgoing BOOLEAN;

UPDATE raeicei.documents
SET is_outgoing = false
WHERE is_outgoing IS NULL;

ALTER TABLE raeicei.documents
    ALTER COLUMN is_outgoing SET NOT NULL;

ALTER TABLE raeicei.documents_aud
    ADD COLUMN IF NOT EXISTS is_outgoing BOOLEAN;

UPDATE raeicei.documents_aud
SET is_outgoing = false
WHERE is_outgoing IS NULL;

ALTER TABLE raeicei.documents_aud
    ALTER COLUMN is_outgoing SET NOT NULL;

ALTER TABLE raeicei.notes
    ADD COLUMN IF NOT EXISTS new_status CHARACTER VARYING (255);

UPDATE raeicei.notes
SET new_status = 'DEFAULT'
WHERE new_status IS NULL;

ALTER TABLE raeicei.notes
    ALTER COLUMN new_status SET NOT NULL;

ALTER TABLE raeicei.notes_aud
    ADD COLUMN IF NOT EXISTS new_status CHARACTER VARYING (255);

UPDATE raeicei.notes_aud
SET new_status = 'DEFAULT'
WHERE new_status IS NULL;

ALTER TABLE raeicei.notes_aud
    ALTER COLUMN new_status SET NOT NULL;