insert into raeicei.eid_manager(id, service_type,create_date, created_by, last_update, updated_by, eik_number,contact, is_active, name, name_latin, logo, home_page, address, client_id, version)
values ('9030e0ed-af59-444e-89e2-1ccda572c372', 'EID_ADMINISTRATOR', now(), 'SYSTEM', now(), 'SYSTEM', '201203809', 'TEST Contact', true, 'МВР', 'MVR',
        'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSOjvLo0ZXYppOsk7wUDpZdBWYdDBTy_SQfVw&s', 'https://www.mvr.bg', 'Гео Милев, ул. „Гео Милев“ 71, 1574 София', null, 1),
       ('86cad23d-5e52-420d-9bee-31bd055a7e5d', 'EID_CENTER', now(), 'SYSTEM', now(), 'SYSTEM', '201203810', 'TEST Contact 2', true, 'Борика', 'Borica', 
	   'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTzRFYHGYb4WOsD1ohzgwDgqk00AwGEW0OWvw&s', 'https://www.borica.bg', 'София 1612 бул. "Цар Борис III" №41', 'eid_borica_app', 1),
       ('af73c15d-573c-4a26-8232-6afc129b145a', 'EID_CENTER', now(), 'SYSTEM', now(), 'SYSTEM', '201203811', 'TEST Contact 3', true, 'Евротръст', 'Evrotrust', 
	   'https://evrotrust.com/build/website/images/evrotrust-logo.png', 'https://evrotrust.com', 'България, София 1766, ул. Околовръстен път 251Г, ММ Бизнес център', 'eid_evrotrust_app', 1);
	      
insert into raeicei.eid_manager_front_office(id, create_date, created_by, last_update, updated_by, contact, is_active,
                                               location, name, region, eid_manager_id, version)
values ('e5c4b51f-8f78-479b-ae83-c9df4bbbfaa2', now(), 'SYSTEM', now(), 'SYSTEM', '0888888881', true, 'Sofia', 'ONLINE',
        'SOFIA', '9030e0ed-af59-444e-89e2-1ccda572c372', 1),
       ('2e81d1f4-1271-4f80-85d2-d55d95deb2e5', now(), 'SYSTEM', now(), 'SYSTEM', '0888888882', true, 'Sofia',
        '03 РУ-СДВР',
        'SOFIA', '9030e0ed-af59-444e-89e2-1ccda572c372', 1),
       ('95a5f6b1-282b-4f5b-bcd0-9ef9cdb2b832', now(), 'SYSTEM', now(), 'SYSTEM', '0888888883', true, 'Sofia',
        '02 РУ-СДВР',
        'SOFIA', '9030e0ed-af59-444e-89e2-1ccda572c372', 1),
		
	   ('72faf82d-2074-450f-beb5-41559190b2e7', now(), 'SYSTEM', now(), 'SYSTEM', '0888888884', true, 'Sofia',
        '05 РУ-СДВР',
        'SOFIA', '86cad23d-5e52-420d-9bee-31bd055a7e5d', 1),
	   ('cbe5b267-5121-4f04-abbb-a8ec2c571d4a', now(), 'SYSTEM', now(), 'SYSTEM', '0888888885', true, 'Sofia',
        '06 РУ-СДВР',
        'SOFIA', 'af73c15d-573c-4a26-8232-6afc129b145a', 1);
        

insert into raeicei.devices(id, create_date, created_by, last_update, updated_by, description, name, type, version)
values ('bc9f97f8-b004-4b61-ac85-7d1a7cb05f14', now(), 'SYSTEM', now(), 'SYSTEM', 'Chip card', 'Карта с чип',
        'CHIP_CARD', 1),
       ('cf2a0594-108d-487f-b588-67033e1a0555', now(), 'SYSTEM', now(), 'SYSTEM', 'Mobile Phone', 'Мобилен телефон',
        'MOBILE', 1),
       ('67a169c1-938e-4b6e-913c-20d85ac586cd', now(), 'SYSTEM', now(), 'SYSTEM', 'Other', 'Друго',
        'OTHER', 1);

insert into raeicei.eid_administrator_device(eid_administrator_id, device_id)
values ('9030e0ed-af59-444e-89e2-1ccda572c372', 'bc9f97f8-b004-4b61-ac85-7d1a7cb05f14'),
       ('9030e0ed-af59-444e-89e2-1ccda572c372', 'cf2a0594-108d-487f-b588-67033e1a0555');
       
insert into raeicei.provided_service(id, create_date, created_by, last_update, updated_by, name, name_latin, service_type, application_type, version)
values ('415fbe9d-3a12-4fbf-9c50-1114056757cc', now(), 'SYSTEM', now(), 'SYSTEM', 'Заявление за издаване', 'Issue Application', 'EID_ADMINISTRATOR', 'ISSUE_EID', 1),
	   ('f8c873f1-5b2f-498d-afda-e00df113aa11', now(), 'SYSTEM', now(), 'SYSTEM', 'Заявление за възобновяване', 'Resume Application', 'EID_ADMINISTRATOR', 'RESUME_EID', 1),
       ('583c199c-fb3a-44aa-8dcf-11b16f1dbb96', now(), 'SYSTEM', now(), 'SYSTEM', 'Заявление за прекратяване', 'Revoke Application', 'EID_ADMINISTRATOR', 'REVOKE_EID', 1),
       ('f52a865b-ea71-454c-b060-8ac454178764', now(), 'SYSTEM', now(), 'SYSTEM', 'Заявление за спиране', 'Stop Application', 'EID_ADMINISTRATOR', 'STOP_EID', 1),
       ('152185f6-65f7-4b8e-b7e7-27103cf9e482', now(), 'SYSTEM', now(), 'SYSTEM', 'Автентикация с EID', 'EID Authentication', 'EID_CENTER', null, 1),
       ('9cccc150-494b-498c-a0ec-4f4145182fea', now(), 'SYSTEM', now(), 'SYSTEM', 'Разширена Автентикация с EID', 'Extented EIdentity Authentication', 'EID_CENTER', null, 1);