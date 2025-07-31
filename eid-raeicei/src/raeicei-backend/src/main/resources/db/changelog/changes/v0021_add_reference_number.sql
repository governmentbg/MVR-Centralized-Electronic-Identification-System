ALTER TABLE raeicei.application
    ADD COLUMN IF NOT EXISTS reference_number CHARACTER VARYING (255);

ALTER TABLE raeicei.application_aud
    ADD COLUMN IF NOT EXISTS reference_number CHARACTER VARYING (255);