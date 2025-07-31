DROP TABLE IF EXISTS raeicei.eid_manager_emploees ;   

CREATE TABLE IF NOT EXISTS raeicei.employee 
    ( 
        id UUID NOT NULL, 
        create_date               TIMESTAMP(6) WITHOUT TIME ZONE, 
        created_by                CHARACTER VARYING(255), 
        last_update               TIMESTAMP(6) WITHOUT TIME ZONE, 
        updated_by                CHARACTER VARYING(255), 
        VERSION                   BIGINT, 
        citizen_identifier_number CHARACTER VARYING(10), 
        citizen_identifier_type   CHARACTER VARYING(255), 
        email                     CHARACTER VARYING(255), 
        is_active                 BOOLEAN, 
        NAME                      CHARACTER VARYING(255) NOT NULL, 
        name_latin                CHARACTER VARYING(255) NOT NULL, 
        phone_number              CHARACTER VARYING(255), 
        PRIMARY KEY (id) );

--ALTER TABLE raeicei.employee DROP CONSTRAINT citizen_identifier_index;        
ALTER TABLE ONLY raeicei.employee
    ADD CONSTRAINT citizen_identifier_index UNIQUE (citizen_identifier_type, citizen_identifier_number);
    
--ALTER TABLE raeicei.employee DROP CONSTRAINT employee_citizen_identifier_type_check;          
ALTER TABLE ONLY raeicei.employee
    ADD CHECK ((citizen_identifier_type)::TEXT = ANY ((ARRAY['EGN'::CHARACTER VARYING, 'LNCh':: CHARACTER VARYING, 'FP'::CHARACTER VARYING])::TEXT[]));

--ALTER TABLE raeicei.eid_manager DROP CONSTRAINT eid_manager_status_check;

ALTER TABLE raeicei.eid_manager
    ADD CONSTRAINT eid_manager_status_check
        CHECK (manager_status IN ('IN_REVIEW', 'ACTIVE', 'DENIED', 'ACCEPTED'));

--ALTER TABLE raeicei.eid_manager_aud DROP CONSTRAINT eid_manager_aud_status_check;

ALTER TABLE raeicei.eid_manager_aud
    ADD CONSTRAINT eid_manager_aud_status_check
        CHECK (manager_status IN ('IN_REVIEW', 'ACTIVE', 'DENIED', 'ACCEPTED'));
    
INSERT INTO raeicei.employee (id, create_date, created_by, last_update, updated_by, version, citizen_identifier_number, citizen_identifier_type, email, is_active, name, name_latin, phone_number) 
VALUES ('b9ba1f0c-f516-42b9-adf3-8eb42d4f7115', '2025-03-25 13:27:20', 'SYSTEM', '2025-03-25 13:27:20', 'SYSTEM', 1, '7010112206', 'EGN', 'test1@mvr.bg', true, 'Тодор Алексиев', 'Todor Aleksiev', '1234561');

INSERT INTO raeicei.employee (id, create_date, created_by, last_update, updated_by, version, citizen_identifier_number, citizen_identifier_type, email, is_active, name, name_latin, phone_number) 
VALUES ('5e35234b-6f4f-4f2a-b113-a1b2067bd0b4', '2025-03-25 13:27:20', 'SYSTEM', '2025-03-25 13:27:20', 'SYSTEM', 1, '8508018457', 'EGN', 'test2@mvr.bg', true, 'Силвия Василева', 'Silviya Vasileva', '1234562');

INSERT INTO raeicei.employee (id, create_date, created_by, last_update, updated_by, version, citizen_identifier_number, citizen_identifier_type, email, is_active, name, name_latin, phone_number) 
VALUES ('093b07ae-1406-4d03-8a78-ecb109869746', '2025-03-25 13:27:20', 'SYSTEM', '2025-03-25 13:27:20', 'SYSTEM', 1, '901020497', 'EGN', 'test3@mvr.bg', true, 'Беатрис Кирилова Алексиева', 'Beatris Kirilova Aleksieva', '1234563');

INSERT INTO raeicei.employee (id, create_date, created_by, last_update, updated_by, version, citizen_identifier_number, citizen_identifier_type, email, is_active, name, name_latin, phone_number) 
VALUES ('ae6e66a0-88d9-4eda-9f54-adf5a168e239', '2025-03-25 13:27:20', 'SYSTEM', '2025-03-25 13:27:20', 'SYSTEM', 1, '8502203020', 'EGN', 'test4@mvr.bg', true, 'Ивайло Каменов Шанков', 'Ivaylo Kamenov Shankova', '1234564');

INSERT INTO raeicei.employee (id, create_date, created_by, last_update, updated_by, version, citizen_identifier_number, citizen_identifier_type, email, is_active, name, name_latin, phone_number) 
VALUES ('748950df-0128-459e-885a-d948f621b359', '2025-03-25 13:27:20', 'SYSTEM', '2025-03-25 13:27:20', 'SYSTEM', 1, '5702027160', 'EGN', 'test5@vr.bg', true, 'Али Халибрям Ахмед', 'Ali Halibryam Ahmed', '1234565');

INSERT INTO raeicei.employee (id, create_date, created_by, last_update, updated_by, version, citizen_identifier_number, citizen_identifier_type, email, is_active, name, name_latin, phone_number) 
VALUES ('472b09e9-45ae-4823-b97d-1b2c63c45d00', '2025-03-25 13:27:20', 'SYSTEM', '2025-03-25 13:27:20', 'SYSTEM', 1, '5407145381', 'EGN', 'test6@mvr.bg', true, 'Искрен Йосифов Йосифов', 'Iskren Yosifov Yosifov', '1234566');

INSERT INTO raeicei.employee (id, create_date, created_by, last_update, updated_by, version, citizen_identifier_number, citizen_identifier_type, email, is_active, name, name_latin, phone_number) 
VALUES ('ba95bbdd-e9c1-4fa1-ad2a-b72a5db78cdc', '2025-03-25 13:27:20', 'SYSTEM', '2025-03-25 13:27:20', 'SYSTEM', 1, '6705083597', 'EGN', 'test7@mvr.bg', true, 'Фатме Ахмед Апти', 'Fatme Ahmed Apti', '1234567');

INSERT INTO raeicei.employee (id, create_date, created_by, last_update, updated_by, version, citizen_identifier_number, citizen_identifier_type, email, is_active, name, name_latin, phone_number) 
VALUES ('0b33ba66-7783-4996-9f76-f42bb5adf35e', '2025-03-25 13:27:20', 'SYSTEM', '2025-03-25 13:27:20', 'SYSTEM', 1, '7008138811', 'EGN', 'test8@mvr.bg', true, 'Анка Стефанова Митева', 'Anka Stefanova Miteva', '1234568');

ALTER TABLE raeicei.discounts ALTER COLUMN online_service SET DEFAULT false;   

INSERT INTO raeicei.discounts (id, create_date, created_by, last_update, updated_by, version, age_from, age_until, disability, start_date, value, eid_manager_id, online_service) VALUES ('2e42f2e8-d221-4353-9473-eb7430e4c37b', '2025-04-14 13:07:52', 'SYS', '2025-04-14 13:07:52', 'SYS', 1, 65, 120, false, '2025-04-01', 50.0, '9030e0ed-af59-444e-89e2-1ccda572c372', false);
INSERT INTO raeicei.discounts (id, create_date, created_by, last_update, updated_by, version, age_from, age_until, disability, start_date, value, eid_manager_id, online_service) VALUES ('a80c853d-b063-4b74-969d-9b0bd597f042', '2025-04-14 13:07:52', 'SYS', '2025-04-14 13:07:52', 'SYS', 1, 13, 120, false, '2025-04-01', 50.0, '9030e0ed-af59-444e-89e2-1ccda572c372', true);
INSERT INTO raeicei.discounts (id, create_date, created_by, last_update, updated_by, version, age_from, age_until, disability, start_date, value, eid_manager_id, online_service) VALUES ('4ac74f78-e056-4bad-8e0a-77501800ee9b', '2025-04-14 13:07:52', 'SYS', '2025-04-14 13:07:52', 'SYS', 1, 14, 35, false, '2025-04-01', 50.0, '9030e0ed-af59-444e-89e2-1ccda572c372', false);

--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', 'f438bd26-328c-4df9-aeae-9c57364a48c0');
--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', 'c4574bf5-495d-4fe3-98b3-7e64279689a5');
--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', '739c20ac-0241-43ef-97cc-f84c585d99cc');
--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', '16c8ff86-09e9-4406-b3d9-eed4f08ea0e3');
--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', 'd3d085da-9f69-44fb-b60c-beaf1ebc6e2a');
--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', 'a9873742-5ee0-4fbd-9126-0f3c1fcbc4f3');
--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', '790f5072-4a9b-482c-98ff-81f2f1349606');
--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', '8ad146c0-62d5-47ea-967a-2f43e83cf09e');
--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', 'c747afe4-8257-4dfc-921c-fd6c16157142');
--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', 'adc5e85d-ab38-415a-9036-209eec02b6af');
--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', '323bf59f-8eb9-4cd6-987f-dffb261f4154');
--INSERT INTO raeicei.eid_manager_offices (eid_manager_id, office_id) VALUES ('9030e0ed-af59-444e-89e2-1ccda572c372', 'b66c9cff-1106-48e9-a265-19e9e8375a40');