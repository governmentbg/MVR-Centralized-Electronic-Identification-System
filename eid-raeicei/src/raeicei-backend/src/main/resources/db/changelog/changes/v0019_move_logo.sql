ALTER TABLE raeicei.eid_manager
    DROP COLUMN IF EXISTS logo;

ALTER TABLE raeicei.eid_manager_aud
    DROP COLUMN IF EXISTS logo;

ALTER TABLE raeicei.application
    ADD COLUMN IF NOT EXISTS logo CHARACTER VARYING (255);

ALTER TABLE raeicei.application_aud
    ADD COLUMN IF NOT EXISTS logo CHARACTER VARYING (255);