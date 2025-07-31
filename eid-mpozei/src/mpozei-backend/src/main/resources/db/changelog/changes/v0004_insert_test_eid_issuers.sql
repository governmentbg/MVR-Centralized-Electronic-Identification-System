insert into mpozei.nomenclature_type (id, name, create_date, created_by, last_update, updated_by)
values ('50ef8d00-a0d3-4af9-bdc7-a23094a9df57', 'EID_ISSUER_TYPE', now(), 'SYSTEM', now(), 'SYSTEM');

insert into mpozei.eid_issuer_nomenclature (id, create_date, created_by, last_update, updated_by, active, description, language, name, nomenclature_type, eid_administrator_id)
values
    ('95918d2b-dd57-41df-9674-fd1a685eac74',now(), 'SYSTEM', now(), 'SYSTEM', true, 'МВР-София','BG','MVR_SOFIA', '50ef8d00-a0d3-4af9-bdc7-a23094a9df57', '9030e0ed-af59-444e-89e2-1ccda572c372' ),
    ('a90db1de-4d84-45c9-a434-71f41fa4010a',now(), 'SYSTEM', now(), 'SYSTEM', true, 'MVR-Sofia','EN','MVR_SOFIA', '50ef8d00-a0d3-4af9-bdc7-a23094a9df57', '9030e0ed-af59-444e-89e2-1ccda572c372' );