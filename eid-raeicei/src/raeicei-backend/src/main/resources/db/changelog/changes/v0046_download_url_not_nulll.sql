
ALTER TABLE raeicei.application
    ALTER COLUMN download_url DROP NOT NULL;

ALTER TABLE raeicei.application_aud
    ALTER COLUMN download_url DROP NOT NULL;

ALTER TABLE raeicei.eid_manager
    ALTER COLUMN download_url DROP NOT NULL;

ALTER TABLE raeicei.eid_manager_aud
    ALTER COLUMN download_url DROP NOT NULL;