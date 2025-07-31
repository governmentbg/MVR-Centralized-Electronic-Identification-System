ALTER TABLE 
   raeicei.eid_manager ADD COLUMN code character varying(3);   
   
update raeicei.eid_manager a
set code = 'МВР'
where a.id = '9030e0ed-af59-444e-89e2-1ccda572c372';  

update raeicei.eid_manager a
set code = 'BR'
where a.id = '86cad23d-5e52-420d-9bee-31bd055a7e5d';  

update raeicei.eid_manager a
set code = 'EVR'
where a.id = 'af73c15d-573c-4a26-8232-6afc129b145a';  

update raeicei.eid_manager a
set code = 'TST'
where a.id = '1884008a-c986-4d9c-b088-b13091eb1709';  

ALTER TABLE 
   raeicei.eid_manager ALTER COLUMN code SET NOT NULL;
   
ALTER TABLE 
   raeicei.eid_manager ADD UNIQUE (code);   






   