UPDATE raeicei.devices
SET type = 'OTHER'
WHERE type NOT IN ('CHIP_CARD', 'MOBILE', 'OTHER');

UPDATE raeicei.devices_aud
SET type = 'OTHER'
WHERE type NOT IN ('CHIP_CARD', 'MOBILE', 'OTHER');

SELECT drop_constraint('raeicei', 'devices', 'type', 'c');
ALTER TABLE raeicei.devices
    ADD CONSTRAINT devices_type_check
        CHECK (type IN ('CHIP_CARD', 'MOBILE', 'OTHER'));

SELECT drop_constraint('raeicei', 'devices_aud', 'type', 'c');
ALTER TABLE raeicei.devices_aud
    ADD CONSTRAINT devices_aud_type_check
        CHECK (type IN ('CHIP_CARD', 'MOBILE', 'OTHER'));

ALTER TABLE raeicei.devices
    ALTER COLUMN type SET NOT NULL;

insert into raeicei.front_office(id, create_date, created_by, last_update, updated_by, contact, is_active,
                                 location, name, region, version, latitude, longitude, working_hours, code, email)
values ('c1f62423-1234-4b6a-b7dc-9876543210ab', now(), 'SYSTEM', now(), 'SYSTEM', '0888888886', true, 'Sofia', 'МЕУ',
        'SOFIA', 1, '41.14', '22.21', '10:00 - 12:00ч.___14:00 - 16:00ч.', 'MEU', 'priemna@mvr.bg');

insert into raeicei.eid_manager_offices(eid_manager_id, office_id)
values ('9030e0ed-af59-444e-89e2-1ccda572c372', 'c1f62423-1234-4b6a-b7dc-9876543210ab');