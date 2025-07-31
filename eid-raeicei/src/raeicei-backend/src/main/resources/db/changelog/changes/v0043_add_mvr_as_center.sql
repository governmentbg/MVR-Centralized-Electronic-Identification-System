insert into raeicei.eid_manager(id, service_type, create_date, created_by, last_update, updated_by, eik_number, manager_status, code, name, name_latin, logo, home_page, address, client_id, version)
values ('d37d5b36-a020-4b8f-8b9d-c3513f6bbe8b', 'EID_CENTER', now(), 'SYSTEM', now(), 'SYSTEM', '201203809', 'ACTIVE', 'KNS', 'МВР', 'MVR',
        'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSOjvLo0ZXYppOsk7wUDpZdBWYdDBTy_SQfVw&s', 'https://www.mvr.bg', 'Гео Милев, ул. „Гео Милев“ 71, 1574 София', 'eid_mvr_app', 1);

insert into raeicei.revinfo(rev, revtstmp)
values ('6005', '1750258959533');

insert into raeicei.eid_manager_aud(id, service_type, eik_number, manager_status, code, name, name_latin, logo, home_page, address, client_id, rev, revtype)
values ('d37d5b36-a020-4b8f-8b9d-c3513f6bbe8b', 'EID_CENTER', '201203809', 'ACTIVE', 'KNS', 'МВР', 'MVR', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSOjvLo0ZXYppOsk7wUDpZdBWYdDBTy_SQfVw&s', 'https://www.mvr.bg', 'Гео Милев, ул. „Гео Милев“ 71, 1574 София', 'eid_mvr_app', 6005, 1);

insert into raeicei.tariffs (id, tariff_type, create_date, created_by, last_update, updated_by, currency, price, start_date, eid_manager_id, device_id, provided_service_id, version, is_active)
values ('675c2199-bc56-4ac5-8f94-eab5f231c0f6', 'SERVICE_TARIFF', now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 1, now(), 'd37d5b36-a020-4b8f-8b9d-c3513f6bbe8b', null, '152185f6-65f7-4b8e-b7e7-27103cf9e482', 1, true), ('73fd4749-cd8c-454c-a4ee-7d3a946c5a8e', 'SERVICE_TARIFF', now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 2, now(), 'd37d5b36-a020-4b8f-8b9d-c3513f6bbe8b', null, '9cccc150-494b-498c-a0ec-4f4145182fea', 1, true);

insert into raeicei.eid_manager_services(eid_manager_id, service_id)
values ('d37d5b36-a020-4b8f-8b9d-c3513f6bbe8b', '152185f6-65f7-4b8e-b7e7-27103cf9e482'), ('d37d5b36-a020-4b8f-8b9d-c3513f6bbe8b', '9cccc150-494b-498c-a0ec-4f4145182fea');