ALTER TABLE 
   raeicei.front_office ADD COLUMN code character varying(4);   
   
update raeicei.front_office a
set code = 'ONL'
where a.id = 'e5c4b51f-8f78-479b-ae83-c9df4bbbfaa2';  
   
update raeicei.front_office a
set code = 'РПУ3'
where a.id = '2e81d1f4-1271-4f80-85d2-d55d95deb2e5';  

update raeicei.front_office a
set code = 'РПУ2'
where a.id = '95a5f6b1-282b-4f5b-bcd0-9ef9cdb2b832';  

update raeicei.front_office a
set code = 'РПУ5'
where a.id = '72faf82d-2074-450f-beb5-41559190b2e7';  

update raeicei.front_office a
set code = 'РПУ6'
where a.id = 'cbe5b267-5121-4f04-abbb-a8ec2c571d4a';  

update raeicei.front_office a
set code = 'АН1'
where a.id = '123441e8-f09e-4df5-91ed-3dfe4af4c64a';  

update raeicei.front_office a
set code = 'АН2'
where a.id = '5088d525-1dc8-4424-b0e8-4c576970647e';  

ALTER TABLE 
   raeicei.front_office ALTER COLUMN code SET NOT NULL;
   
ALTER TABLE 
   raeicei.front_office ADD UNIQUE (code);   