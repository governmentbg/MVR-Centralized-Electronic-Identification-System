
insert into raeicei.provided_service(id, create_date, created_by, last_update, updated_by, name, name_latin, service_type, version)
values ('bebf06aa-2781-49bf-ba2c-3cab23ad8b92', now(), 'SYSTEM', now(), 'SYSTEM', 'Издаване на носител', 'Issue of carrier', 'EID_ADMINISTRATOR', 1);

insert into raeicei.tariffs (id, tariff_type, create_date, created_by, last_update, updated_by, currency, price, start_date, eid_manager_id, device_id, provided_service_id, version)
values 
	   ('37ad2003-0683-4b0e-b6be-91cfefbcee68', 'SERVICE_TARIFF', now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 100, now(), '9030e0ed-af59-444e-89e2-1ccda572c372', null, 
	   '415fbe9d-3a12-4fbf-9c50-1114056757cc', 1),
       ('1748256f-82e0-4078-834b-285f8581ec76', 'SERVICE_TARIFF', now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 90, now(), '9030e0ed-af59-444e-89e2-1ccda572c372' , null, 
	   'f8c873f1-5b2f-498d-afda-e00df113aa11', 1),
       ('33fdc20e-e914-4c91-9dee-d485011044a3', 'SERVICE_TARIFF', now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 80, now(), '9030e0ed-af59-444e-89e2-1ccda572c372' , null, 
	   '583c199c-fb3a-44aa-8dcf-11b16f1dbb96', 1),	
       ('2fa96093-3502-48bd-bc6b-8889dbb87ce8', 'SERVICE_TARIFF', now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 70, now(), '9030e0ed-af59-444e-89e2-1ccda572c372' , null, 
	   'f52a865b-ea71-454c-b060-8ac454178764', 1),
	   
       ('acd80878-a5b4-473f-8b9a-efe3f76f732c', 'SERVICE_TARIFF', now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 60, now(), '86cad23d-5e52-420d-9bee-31bd055a7e5d' , null,
	    '9cccc150-494b-498c-a0ec-4f4145182fea', 1),
       ('437ff396-4164-48ce-a0be-6cfbd451e6ea', 'SERVICE_TARIFF', now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 50, now(), '86cad23d-5e52-420d-9bee-31bd055a7e5d' , null,
	    '152185f6-65f7-4b8e-b7e7-27103cf9e482', 1),
	   ('78f9ead7-21e3-43ed-826a-36e7259ceb11', 'SERVICE_TARIFF', now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 40, now(), 'af73c15d-573c-4a26-8232-6afc129b145a' , null,
	    '9cccc150-494b-498c-a0ec-4f4145182fea', 1),
       ('5b81907d-0efa-4787-954b-988c8150c72b', 'SERVICE_TARIFF', now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 30, now(), 'af73c15d-573c-4a26-8232-6afc129b145a' , null,
	    '152185f6-65f7-4b8e-b7e7-27103cf9e482', 1),
	   
       ('63491bbf-5d1c-48bd-b1a9-f7108825fb5b', 'DEVICE_TARIFF' , now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 20, now(), '9030e0ed-af59-444e-89e2-1ccda572c372',  'bc9f97f8-b004-4b61-ac85-7d1a7cb05f14',
	    'bebf06aa-2781-49bf-ba2c-3cab23ad8b92', 1),
	   ('a1dd5f24-ca71-4ee3-8816-003b69344bbf', 'DEVICE_TARIFF' , now(), 'SYSTEM', now(), 'SYSTEM', 'BGN', 10, now(), '9030e0ed-af59-444e-89e2-1ccda572c372',  'cf2a0594-108d-487f-b588-67033e1a0555',
	    'bebf06aa-2781-49bf-ba2c-3cab23ad8b92', 1)
;
