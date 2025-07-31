ALTER TABLE raeicei.discounts
    ADD COLUMN IF NOT EXISTS currency character varying (3);

ALTER TABLE raeicei.discounts
    ADD CONSTRAINT discount_currency_check
        CHECK (currency IN ('BGN', 'EUR'));

UPDATE raeicei.discounts
SET currency = 'BGN'
WHERE currency IS NULL;

ALTER TABLE raeicei.discounts
    ALTER COLUMN currency SET NOT NULL;

ALTER TABLE raeicei.discounts_aud
    ADD COLUMN IF NOT EXISTS currency character varying (3);

ALTER TABLE raeicei.discounts_aud
    ADD CONSTRAINT discount_aud_currency_check
        CHECK (currency IN ('BGN', 'EUR'));

UPDATE raeicei.discounts_aud
SET currency = 'BGN'
WHERE currency IS NULL;

ALTER TABLE raeicei.discounts_aud
    ALTER COLUMN currency SET NOT NULL;