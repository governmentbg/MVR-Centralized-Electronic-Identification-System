CREATE TABLE IF NOT EXISTS
    eid_manager_authorized_persons
(
    eid_manager_id UUID NOT NULL,
    authorized_person_id UUID NOT NULL,
    CONSTRAINT fkhytphbd4538gas6d93wv28had FOREIGN KEY (authorized_person_id) REFERENCES
        "contact" ("id"),
    CONSTRAINT fkpdysph770hly0qrucdyybusox FOREIGN KEY (eid_manager_id) REFERENCES
        "eid_manager" ("id"),
    CONSTRAINT ukgbpfobcl4bom97go13nh3bp0 UNIQUE (authorized_person_id)
);

ALTER TABLE raeicei.eid_manager
DROP COLUMN IF EXISTS authorized_person;

ALTER TABLE raeicei.eid_manager_aud
DROP COLUMN IF EXISTS authorized_person;

ALTER TABLE raeicei.eid_manager
DROP CONSTRAINT IF EXISTS uk_m7vvtost7dyp0wamvgppo3ite;

ALTER TABLE raeicei.eid_manager
DROP CONSTRAINT IF EXISTS uk_nemtaf91d1rsrmugplv915joc;

ALTER TABLE raeicei.eid_manager
DROP CONSTRAINT IF EXISTS unique_eik_number;

ALTER TABLE raeicei.eid_manager
ADD CONSTRAINT uq_eidmgr_eik_service
UNIQUE (eik_number, service_type);

ALTER TABLE raeicei.eid_manager
DROP COLUMN IF EXISTS contact;

ALTER TABLE raeicei.eid_manager_aud
DROP COLUMN IF EXISTS contact;

ALTER TABLE raeicei.eid_manager
DROP CONSTRAINT IF EXISTS eid_manager_status_check;

ALTER TABLE raeicei.eid_manager
ADD CONSTRAINT eid_manager_status_check
CHECK (manager_status IN ('ACTIVE', 'SUSPENDED', 'STOP'));

ALTER TABLE raeicei.eid_manager_aud
DROP CONSTRAINT IF EXISTS eid_manager_aud_status_check;

ALTER TABLE raeicei.eid_manager_aud
ADD CONSTRAINT eid_manager_aud_status_check
CHECK (manager_status IN ('ACTIVE', 'SUSPENDED', 'STOP'));