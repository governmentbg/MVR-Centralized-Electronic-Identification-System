ALTER TABLE raeicei.discounts
    ADD COLUMN IF NOT EXISTS is_active BOOLEAN;

UPDATE raeicei.discounts
SET is_active = true
WHERE is_active IS NULL;

ALTER TABLE raeicei.discounts_aud
    ADD COLUMN IF NOT EXISTS is_active BOOLEAN;

ALTER TABLE raeicei.provided_service
    ADD COLUMN IF NOT EXISTS is_active BOOLEAN;

UPDATE raeicei.provided_service
SET is_active = true
WHERE is_active IS NULL;

ALTER TABLE raeicei.provided_service_aud
    ADD COLUMN IF NOT EXISTS is_active BOOLEAN;

ALTER TABLE raeicei.tariffs
    ADD COLUMN IF NOT EXISTS is_active BOOLEAN;

UPDATE raeicei.tariffs
SET is_active = true
WHERE is_active IS NULL;

ALTER TABLE raeicei.tariffs_aud
    ADD COLUMN IF NOT EXISTS is_active BOOLEAN;