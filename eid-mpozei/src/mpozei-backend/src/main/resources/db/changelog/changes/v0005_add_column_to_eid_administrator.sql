alter table mpozei.eid_administrator
    add column if not exists eik_number varchar(255);

update mpozei.eid_administrator
set eik_number = '1234567890000'
where mpozei.eid_administrator.id = '9030e0ed-af59-444e-89e2-1ccda572c372';