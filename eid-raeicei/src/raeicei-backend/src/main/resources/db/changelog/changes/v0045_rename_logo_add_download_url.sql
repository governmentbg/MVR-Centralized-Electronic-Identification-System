ALTER TABLE raeicei.application
    RENAME COLUMN logo TO logo_url;

ALTER TABLE raeicei.application_aud
    RENAME COLUMN logo TO logo_url;

ALTER TABLE raeicei.eid_manager
    RENAME COLUMN logo TO logo_url;

ALTER TABLE raeicei.eid_manager_aud
    RENAME COLUMN logo TO logo_url;

ALTER TABLE raeicei.application
    ADD COLUMN IF NOT EXISTS download_url CHARACTER VARYING(255);

UPDATE raeicei.application
SET download_url = 'EMPTY'
WHERE download_url IS NULL;

ALTER TABLE raeicei.application
    ALTER COLUMN download_url SET NOT NULL;

ALTER TABLE raeicei.application_aud
    ADD COLUMN IF NOT EXISTS download_url CHARACTER VARYING(255);

UPDATE raeicei.application_aud
SET download_url = 'EMPTY'
WHERE download_url IS NULL;

ALTER TABLE raeicei.application_aud
    ALTER COLUMN download_url SET NOT NULL;

ALTER TABLE raeicei.eid_manager
    ADD COLUMN IF NOT EXISTS download_url CHARACTER VARYING(255);

UPDATE raeicei.eid_manager
SET download_url = 'EMPTY'
WHERE download_url IS NULL;

ALTER TABLE raeicei.eid_manager
    ALTER COLUMN download_url SET NOT NULL;

ALTER TABLE raeicei.eid_manager_aud
    ADD COLUMN IF NOT EXISTS download_url CHARACTER VARYING(255);

UPDATE raeicei.eid_manager_aud
SET download_url = 'EMPTY'
WHERE download_url IS NULL;

ALTER TABLE raeicei.eid_manager_aud
    ALTER COLUMN download_url SET NOT NULL;