ALTER TABLE raeicei.application
DROP
CONSTRAINT application_status_check;

ALTER TABLE raeicei.application
    ADD CONSTRAINT application_status_check
        CHECK (status IN ('IN_REVIEW', 'ACTIVE', 'DENIED', 'RETURNED_FOR_CORRECTION', 'ACCEPTED'));

ALTER TABLE raeicei.application_aud
DROP
CONSTRAINT application_aud_status_check;

ALTER TABLE raeicei.application_aud
    ADD CONSTRAINT application_aud_status_check
        CHECK (status IN ('IN_REVIEW', 'ACTIVE', 'DENIED', 'RETURNED_FOR_CORRECTION', 'ACCEPTED'));

ALTER TABLE
    raeicei.discounts
    ADD COLUMN IF NOT EXISTS online_service BOOLEAN;

ALTER TABLE
    raeicei.discounts_aud
    ADD COLUMN IF NOT EXISTS online_service BOOLEAN;