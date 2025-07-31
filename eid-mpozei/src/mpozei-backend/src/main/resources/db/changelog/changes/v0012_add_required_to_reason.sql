alter table mpozei.reason_nomenclature
    add column if not exists text_required boolean default false;

update mpozei.reason_nomenclature rn
set text_required = true
where rn.name in ('STOPPED_OTHER',
                  'STOPPED_BY_ADMINISTRATOR',
                  'RESUMED_OTHER',
                  'REVOKED_OTHER',
                  'REVOKED_BY_ADMINISTRATOR',
                  'DENIED_OTHER',
                  'DENIED_BY_ADMINISTRATOR')