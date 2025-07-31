insert into raeicei.document_types(id, active, name, required_for_administrator, required_for_center)
values ('4f84a716-3a72-4f44-8b0b-69e4a7648e34', true, 'Пълномощно', true, true),
       ('7adfa546-992f-4c27-a1fd-ef505a9b4d60', true, 'Финансови справки', true, true),
       ('bd3e28cd-fcdc-443e-9319-5e4cf56cd15b', true, 'Доказателство за квалификация', true, true),
       ('5b9889ec-b74b-4e6d-80a6-79265ff0a6a3', true, 'Списък с оператори', true, false),
       ('c86ea91e-0411-4f08-9468-2dc4cb1d1e48', true, 'Сертификати', true, true),
       ('8e9fd6bb-b28f-4c7f-9aa6-9c15c8d947dc', true, 'Tехнически документи', true, true),
       ('1705a7fa-0dfb-44ee-91ff-d1e2c1c6d1b7', true, 'Протокол техническа свързаност', true, true),
       ('f2e4cd98-3ebf-4195-9ed3-0280f1cb9a31', true, 'Заповед', true, true),
       ('3b2372c8-13cf-41a6-b3c3-4c7c1e1655b3', true, 'Методи за управление на сигурността', true, true),
       ('2c01f1ab-cdc4-4e58-9d4a-984942b6c1b6', true, 'Информация за работни станции', true, true) ON CONFLICT (id) DO
UPDATE SET
    name = EXCLUDED.name,
    active = EXCLUDED.active,
    required_for_administrator = EXCLUDED.required_for_administrator,
    required_for_center = EXCLUDED.required_for_center;


insert into raeicei.document_descriptions(document_id, description, language)
values ('4f84a716-3a72-4f44-8b0b-69e4a7648e34', 'Power of Attorney', 0),
       ('4f84a716-3a72-4f44-8b0b-69e4a7648e34', 'Пълномощно', 1),
       ('7adfa546-992f-4c27-a1fd-ef505a9b4d60', 'Financial Statements', 0),
       ('7adfa546-992f-4c27-a1fd-ef505a9b4d60', 'Финансови справки', 1),
       ('bd3e28cd-fcdc-443e-9319-5e4cf56cd15b', 'Proof of Qualification', 0),
       ('bd3e28cd-fcdc-443e-9319-5e4cf56cd15b', 'Доказателство за квалификация', 1),
       ('5b9889ec-b74b-4e6d-80a6-79265ff0a6a3', 'List of Operators', 0),
       ('5b9889ec-b74b-4e6d-80a6-79265ff0a6a3', 'Списък с оператори', 1),
       ('c86ea91e-0411-4f08-9468-2dc4cb1d1e48', 'Certificates', 0),
       ('c86ea91e-0411-4f08-9468-2dc4cb1d1e48', 'Сертификати', 1),
       ('8e9fd6bb-b28f-4c7f-9aa6-9c15c8d947dc', 'Technical Documents', 0),
       ('8e9fd6bb-b28f-4c7f-9aa6-9c15c8d947dc', 'Tехнически документи', 1),
       ('1705a7fa-0dfb-44ee-91ff-d1e2c1c6d1b7', 'Technical Connectivity Protocol', 0),
       ('1705a7fa-0dfb-44ee-91ff-d1e2c1c6d1b7', 'Протокол техническа свързаност', 1),
       ('f2e4cd98-3ebf-4195-9ed3-0280f1cb9a31', 'Directive', 0),
       ('f2e4cd98-3ebf-4195-9ed3-0280f1cb9a31', 'Заповед', 1),
       ('3b2372c8-13cf-41a6-b3c3-4c7c1e1655b3', 'Security Management Methods', 0),
       ('3b2372c8-13cf-41a6-b3c3-4c7c1e1655b3', 'Методи за управление на сигурността', 1),
       ('2c01f1ab-cdc4-4e58-9d4a-984942b6c1b6', 'Workstations Information', 0),
       ('2c01f1ab-cdc4-4e58-9d4a-984942b6c1b6', 'Информация за работни станции', 1) ON CONFLICT (document_id, language) DO
UPDATE SET
    description = EXCLUDED.description;