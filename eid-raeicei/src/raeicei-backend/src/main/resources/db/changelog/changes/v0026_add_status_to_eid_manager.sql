ALTER TABLE raeicei.eid_manager
DROP CONSTRAINT IF EXISTS eid_manager_status_check;

ALTER TABLE raeicei.eid_manager
    ADD CONSTRAINT eid_manager_status_check
        CHECK (manager_status IN ('ACTIVE', 'SUSPENDED', 'STOP', 'IN_REVIEW'));

ALTER TABLE raeicei.eid_manager_aud
DROP CONSTRAINT IF EXISTS eid_manager_aud_status_check;

ALTER TABLE raeicei.eid_manager_aud
    ADD CONSTRAINT eid_manager_aud_status_check
        CHECK (manager_status IN ('ACTIVE', 'SUSPENDED', 'STOP', 'IN_REVIEW'));