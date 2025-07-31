--DROP TABLE IF EXISTS raeicei.eid_manager_provided_service; 
--DROP TABLE IF EXISTS raeicei.eid_manager_provided_service_aud; 

insert into raeicei.eid_manager(id, service_type,create_date, created_by, last_update, updated_by, eik_number,contact, is_active, name, name_latin, logo, home_page, address, client_id, version)
values ('1884008a-c986-4d9c-b088-b13091eb1709', 'EID_ADMINISTRATOR', now(), 'SYSTEM', now(), 'SYSTEM', '201203111', 'TEST Contact 4', true, 'АЕИ Админ', 'EID Admin',
        'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQCMiHV9D-4zT6PXcnpmljkMNcI_9-5D76OyQ&s', 'https://en.wikipedia.org/wiki/Main_Page', 'София център, ул. „Московска“ 49, 1000 София', null, 1);
	      
insert into raeicei.eid_manager_front_office(id, create_date, created_by, last_update, updated_by, contact, is_active,
                                               location, name, region, eid_manager_id, version)
values ('123441e8-f09e-4df5-91ed-3dfe4af4c64a', now(), 'SYSTEM', now(), 'SYSTEM', '0888888888', true, 'Sofia', 'АНА 1',
        'SOFIA', '1884008a-c986-4d9c-b088-b13091eb1709', 1),
       ('5088d525-1dc8-4424-b0e8-4c576970647e', now(), 'SYSTEM', now(), 'SYSTEM', '0888888889', true, 'Sofia', 'АНА 2',
        'SOFIA', '1884008a-c986-4d9c-b088-b13091eb1709', 1);
        
update raeicei.eid_manager_front_office a
set longitude = '23.2524176',
    latitude = '42.6710201',
    working_hours = '10:00 - 12:00ч.___14:00 - 20:00ч.',
    email = 'priemna@random.bg',
    version = 4
where a.id = '123441e8-f09e-4df5-91ed-3dfe4af4c64a';

update raeicei.eid_manager_front_office a
set longitude = '23.3073087',
    latitude = '42.6633226',
    working_hours = '10:00 - 12:00ч.___14:00 - 20:00ч.',
    email = 'priemna@random.bg',
    version = 4
where a.id = '5088d525-1dc8-4424-b0e8-4c576970647e';
        
        
insert into raeicei.eid_administrator_device(eid_administrator_id, device_id)
values ('1884008a-c986-4d9c-b088-b13091eb1709', 'cf2a0594-108d-487f-b588-67033e1a0555'),
       ('1884008a-c986-4d9c-b088-b13091eb1709', '67a169c1-938e-4b6e-913c-20d85ac586cd');