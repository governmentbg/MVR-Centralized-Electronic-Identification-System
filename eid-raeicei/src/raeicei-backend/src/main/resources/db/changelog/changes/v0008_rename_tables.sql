ALTER TABLE ONLY raeicei.eid_manager_front_office_aud
  RENAME TO front_office_aud;

ALTER TABLE ONLY raeicei.eid_manager_front_office
  RENAME TO front_office;
  
ALTER TABLE 
   raeicei.front_office DROP COLUMN eid_manager_id;

ALTER TABLE 
   raeicei.front_office_aud DROP COLUMN eid_manager_id;   

CREATE TABLE eid_manager_offices (eid_manager_id UUID, office_id UUID NOT NULL, PRIMARY KEY (eid_manager_id, office_id), CONSTRAINT fkkphfwy9o7ggpgwmce2tkjqdp4 FOREIGN KEY (eid_manager_id) REFERENCES "eid_manager" ("id"), CONSTRAINT fklvklwrkiv5fh34nap1wy5iio2 FOREIGN KEY (office_id) REFERENCES "front_office" ("id"));

insert into raeicei.eid_manager_offices(office_id, eid_manager_id)
values ('e5c4b51f-8f78-479b-ae83-c9df4bbbfaa2', '9030e0ed-af59-444e-89e2-1ccda572c372'),
       ('2e81d1f4-1271-4f80-85d2-d55d95deb2e5','9030e0ed-af59-444e-89e2-1ccda572c372'),
       ('95a5f6b1-282b-4f5b-bcd0-9ef9cdb2b832','9030e0ed-af59-444e-89e2-1ccda572c372'),
	   ('72faf82d-2074-450f-beb5-41559190b2e7', '86cad23d-5e52-420d-9bee-31bd055a7e5d'),
	   ('cbe5b267-5121-4f04-abbb-a8ec2c571d4a', 'af73c15d-573c-4a26-8232-6afc129b145a');