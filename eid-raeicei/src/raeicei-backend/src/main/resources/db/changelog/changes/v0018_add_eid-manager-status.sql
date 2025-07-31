ALTER TABLE
    raeicei.eid_manager
    ADD COLUMN IF NOT EXISTS manager_status CHARACTER VARYING (255);

UPDATE raeicei.eid_manager
SET manager_status = 'ACTIVE'
WHERE manager_status IS NULL;

ALTER TABLE
    raeicei.eid_manager_aud
    ADD COLUMN IF NOT EXISTS manager_status CHARACTER VARYING (255);

UPDATE raeicei.eid_manager_aud
SET manager_status = 'ACTIVE'
WHERE manager_status IS NULL;

ALTER TABLE raeicei.eid_manager DROP COLUMN IF EXISTS is_active;

ALTER TABLE raeicei.eid_manager_aud DROP COLUMN IF EXISTS is_active;

--ALTER TABLE raeicei.provided_service DROP CONSTRAINT fkoee95can8bd7pxmma7yohwt8w;

ALTER TABLE raeicei.provided_service DROP COLUMN IF EXISTS manager_id;

ALTER TABLE raeicei.provided_service_aud DROP COLUMN IF EXISTS manager_id;

CREATE TABLE IF NOT EXISTS eid_manager_services
(
    eid_manager_id UUID NOT NULL,
    service_id UUID NOT NULL,
    PRIMARY KEY (eid_manager_id, service_id)
);

ALTER TABLE raeicei.eid_manager_services
    ADD CONSTRAINT FK_manager_provided_service FOREIGN KEY (eid_manager_id) REFERENCES raeicei.eid_manager(id);

ALTER TABLE raeicei.eid_manager_services
    ADD CONSTRAINT FK_provided_service_manager FOREIGN KEY (service_id) REFERENCES raeicei.provided_service(id);

CREATE TABLE IF NOT EXISTS jt_eid_manager_services_aud
(
    eid_manager_id UUID NOT NULL,
    service_id UUID NOT NULL,
    PRIMARY KEY (eid_manager_id, service_id)
);

--ALTER TABLE raeicei.jt_eid_manager_services_aud ADD CONSTRAINT FK_manager_provided_service FOREIGN KEY (eid_manager_id) REFERENCES raeicei.eid_manager_aud(id);

--ALTER TABLE raeicei.jt_eid_manager_services_aud ADD CONSTRAINT FK_provided_service_manager FOREIGN KEY (service_id) REFERENCES raeicei.provided_service_aud(id);