-- Deletes RESUME reason_id and reason_text from Application
UPDATE mpozei.application
SET reason_id = null, reason_text = null
where reason_id in (
	select rn.id from mpozei.reason_nomenclature rn where rn.nomenclature_type='568fb1ec-d894-4202-957e-0caf3a506a59'
);

-- Deletes RESUME from reason_nomenclature
DELETE from mpozei.reason_nomenclature rn
where rn.nomenclature_type='568fb1ec-d894-4202-957e-0caf3a506a59';


-- Deletes RESUME from nomenclature_type
DELETE from mpozei.nomenclature_type nt
where nt.id = '568fb1ec-d894-4202-957e-0caf3a506a59';
