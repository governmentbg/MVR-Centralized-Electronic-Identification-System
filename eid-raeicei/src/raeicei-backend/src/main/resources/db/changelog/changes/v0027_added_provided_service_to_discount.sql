ALTER TABLE raeicei.discounts
    ADD COLUMN IF NOT EXISTS provided_service_id UUID;

ALTER TABLE raeicei.discounts_aud
    ADD COLUMN IF NOT EXISTS provided_service_id UUID;