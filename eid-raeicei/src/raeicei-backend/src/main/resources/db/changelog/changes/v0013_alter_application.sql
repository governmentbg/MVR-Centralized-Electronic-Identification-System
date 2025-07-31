CREATE TABLE  IF NOT EXISTS application_number ( 
        id               CHARACTER VARYING(255) NOT NULL, 
        application_type CHARACTER VARYING(255), 
        PRIMARY KEY (id)
, 
        CHECK ((application_type)::TEXT = ANY ((ARRAY['REGISTER'::CHARACTER VARYING, 'RESUME':: 
        CHARACTER VARYING, 'REVOKE'::CHARACTER VARYING, 'STOP'::CHARACTER VARYING])::TEXT[])) 
);

CREATE TABLE  IF NOT EXISTS contact ( 
        id UUID NOT NULL, 
        create_date               TIMESTAMP(6) WITHOUT TIME ZONE, 
        created_by                CHARACTER VARYING(255), 
        last_update               TIMESTAMP(6) WITHOUT TIME ZONE, 
        updated_by                CHARACTER VARYING(255), 
        VERSION                   BIGINT, 
        citizen_identifier_number CHARACTER VARYING(10), 
        citizen_identifier_type   CHARACTER VARYING(255), 
        email                     CHARACTER VARYING(255), 
        is_active                 BOOLEAN, 
        NAME                      CHARACTER VARYING(255) NOT NULL, 
        name_latin                CHARACTER VARYING(255) NOT NULL, 
        phone_number              CHARACTER VARYING(255), 
        e_identity UUID, 
        PRIMARY KEY (id), 
        CHECK ((citizen_identifier_type)::TEXT = ANY ((ARRAY['EGN'::CHARACTER VARYING, 'LNCh':: 
        CHARACTER VARYING, 'FP'::CHARACTER VARYING])::TEXT[])) 
);

CREATE TABLE  IF NOT EXISTS contact_aud ( 
        id UUID NOT NULL, 
        rev                       INTEGER NOT NULL, 
        revtype                   SMALLINT, 
        citizen_identifier_number CHARACTER VARYING(10), 
        citizen_identifier_type   CHARACTER VARYING(255), 
        email                     CHARACTER VARYING(255), 
        is_active                 BOOLEAN, 
        NAME                      CHARACTER VARYING(255), 
        name_latin                CHARACTER VARYING(255), 
        phone_number              CHARACTER VARYING(255), 
        e_identity UUID, 
        PRIMARY KEY (rev, id), 
        CONSTRAINT fkax6dmwbixsgvotyxv31vs1u3y FOREIGN KEY (rev) REFERENCES "revinfo" ("rev"), 
        CHECK ((citizen_identifier_type)::TEXT = ANY ((ARRAY['EGN'::CHARACTER VARYING, 'LNCh':: 
        CHARACTER VARYING, 'FP'::CHARACTER VARYING])::TEXT[])) 
);

CREATE TABLE  IF NOT EXISTS application 
    ( 
        service_type CHARACTER VARYING(31) NOT NULL, 
        id UUID NOT NULL, 
        create_date        TIMESTAMP(6) WITHOUT TIME ZONE, 
        created_by         CHARACTER VARYING(255), 
        last_update        TIMESTAMP(6) WITHOUT TIME ZONE, 
        updated_by         CHARACTER VARYING(255), 
        VERSION            BIGINT, 
        address            CHARACTER VARYING(255), 
        application_type   CHARACTER VARYING(255), 
        code               CHARACTER VARYING(3) NOT NULL, 
        company_name       CHARACTER VARYING(255) NOT NULL, 
        company_name_latin CHARACTER VARYING(255), 
        description        CHARACTER VARYING(255), 
        eik_number         CHARACTER VARYING(255) NOT NULL, 
        email              CHARACTER VARYING(255), 
        home_page          CHARACTER VARYING(255), 
        phone              CHARACTER VARYING(255), 
        pipeline_status    CHARACTER VARYING(255), 
        status             CHARACTER VARYING(255), 
        applicant UUID, 
        application_number CHARACTER VARYING(255), 
        PRIMARY KEY (id)
, 
        CONSTRAINT fk3tdal2ahus1688bxo5asmclsd FOREIGN KEY (applicant) REFERENCES "contact" ("id"), 
        CONSTRAINT fk7904qmbrt0xvgl0mkvyg431xr FOREIGN KEY (application_number) REFERENCES 
        "application_number" ("id"), 
        CONSTRAINT ukf411obukou1pnwnivf17rvry1 UNIQUE (application_number), 
        CONSTRAINT uk71pwmt7926snas9ph942knkme UNIQUE (code), 
        CHECK ((application_type)::TEXT = ANY ((ARRAY['REGISTER'::CHARACTER VARYING, 'RESUME':: 
        CHARACTER VARYING, 'REVOKE'::CHARACTER VARYING, 'STOP'::CHARACTER VARYING])::TEXT[])), 
        CHECK ((pipeline_status)::TEXT = ANY ((ARRAY['INITIATED'::CHARACTER VARYING, 
        'EJBCA_CERTIFICATE_CREATION'::   CHARACTER VARYING, 'REGIX_VERIFICATION'::CHARACTER VARYING, 
        'REI_EIDENTITY_CREATION'::        CHARACTER VARYING, 'REI_EIDENTITY_VERIFICATION'::CHARACTER 
        VARYING, 'RUEI_BASE_PROFILE_ATTACHMENT'::        CHARACTER VARYING, 
        'RUEI_BASE_PROFILE_VERIFICATION'::          CHARACTER VARYING, 'RUEI_CERTIFICATE_CREATION':: 
                                                       CHARACTER VARYING, 'SIGNATURE_VERIFICATION':: 
                                                  CHARACTER VARYING, 'SIGNATURE_CREATION'::CHARACTER 
        VARYING, 'RUEI_CERTIFICATE_STATUS_VERIFICATION'::CHARACTER VARYING, 
        'RUEI_CERTIFICATE_RETRIEVAL'::                   CHARACTER VARYING, 
        'EJBCA_CERTIFICATE_REVOCATION'::                 CHARACTER VARYING, 
        'RUEI_CHANGE_EID_STATUS'::                       CHARACTER VARYING, 
        'EJBCA_CERTIFICATE_RETRIEVAL'::                  CHARACTER VARYING, 
        'EJBCA_END_ENTITY_CREATION'::                    CHARACTER VARYING, 
        'PUN_CERTIFICATE_CREATION'::                     CHARACTER VARYING, 'PUN_CARRIER_RETRIEVAL' 
        ::                                               CHARACTER VARYING, 'ISSUE_EID_SIGNED':: 
                                                         CHARACTER VARYING, 'SEND_NOTIFICATION':: 
                                                         CHARACTER VARYING, 
        'CERTIFICATE_HISTORY_CREATION'::                 CHARACTER VARYING, 'PIVR_VERIFICATION':: 
                                                         CHARACTER VARYING, 'EXPORT_APPLICATION':: 
                                                         CHARACTER VARYING, 'DENIED_APPLICATION':: 
                                                         CHARACTER VARYING, 
        'E_DELIVERY_NOTIFICATION'::                      CHARACTER VARYING, 'XML_TIMESTAMP':: 
                                                         CHARACTER VARYING, 'CREATE_PAYMENT':: 
                                                         CHARACTER VARYING, 'CALCULATE_PAYMENT':: 
                                                         CHARACTER VARYING, 
        'EXISTING_CERTIFICATE_VERIFICATION'::            CHARACTER VARYING])::TEXT[])), 
        CHECK ((status)::TEXT = ANY ((ARRAY['SUBMITTED'::CHARACTER VARYING, 'PROCESSING'::CHARACTER 
        VARYING, 'PENDING_SIGNATURE'::CHARACTER VARYING, 'SIGNED'::CHARACTER VARYING, 
        'PENDING_PAYMENT'::           CHARACTER VARYING, 'PAYMENT_EXPIRED'::CHARACTER VARYING, 
        'PAID'::                      CHARACTER VARYING, 'DENIED'::CHARACTER VARYING, 'APPROVED':: 
                                      CHARACTER VARYING, 'GENERATED_CERTIFICATE'::CHARACTER VARYING 
        , 'COMPLETED'::               CHARACTER VARYING, 'CERTIFICATE_STORED'::CHARACTER VARYING]) 
        ::TEXT[])) 
);

CREATE TABLE  IF NOT EXISTS application_aud ( 
        id UUID NOT NULL, 
        rev                INTEGER NOT NULL, 
        service_type       CHARACTER VARYING(31) NOT NULL, 
        revtype            SMALLINT, 
        address            CHARACTER VARYING(255), 
        application_type   CHARACTER VARYING(255), 
        code               CHARACTER VARYING(3), 
        company_name       CHARACTER VARYING(255), 
        company_name_latin CHARACTER VARYING(255), 
        description        CHARACTER VARYING(255), 
        eik_number         CHARACTER VARYING(255), 
        email              CHARACTER VARYING(255), 
        home_page          CHARACTER VARYING(255), 
        phone              CHARACTER VARYING(255), 
        pipeline_status    CHARACTER VARYING(255), 
        status             CHARACTER VARYING(255), 
        applicant UUID, 
        PRIMARY KEY (rev, id), 
        CONSTRAINT fkp908xu879qljr5nc0eq2pc7na FOREIGN KEY (rev) REFERENCES "revinfo" ("rev"), 
        CHECK ((application_type)::TEXT = ANY ((ARRAY['REGISTER'::CHARACTER VARYING, 'RESUME':: 
        CHARACTER VARYING, 'REVOKE'::CHARACTER VARYING, 'STOP'::CHARACTER VARYING])::TEXT[])), 
        CHECK ((pipeline_status)::TEXT = ANY ((ARRAY['INITIATED'::CHARACTER VARYING, 
        'EJBCA_CERTIFICATE_CREATION'::   CHARACTER VARYING, 'REGIX_VERIFICATION'::CHARACTER VARYING, 
        'REI_EIDENTITY_CREATION'::        CHARACTER VARYING, 'REI_EIDENTITY_VERIFICATION'::CHARACTER 
        VARYING, 'RUEI_BASE_PROFILE_ATTACHMENT'::        CHARACTER VARYING, 
        'RUEI_BASE_PROFILE_VERIFICATION'::          CHARACTER VARYING, 'RUEI_CERTIFICATE_CREATION':: 
                                                       CHARACTER VARYING, 'SIGNATURE_VERIFICATION':: 
                                                  CHARACTER VARYING, 'SIGNATURE_CREATION'::CHARACTER 
        VARYING, 'RUEI_CERTIFICATE_STATUS_VERIFICATION'::CHARACTER VARYING, 
        'RUEI_CERTIFICATE_RETRIEVAL'::                   CHARACTER VARYING, 
        'EJBCA_CERTIFICATE_REVOCATION'::                 CHARACTER VARYING, 
        'RUEI_CHANGE_EID_STATUS'::                       CHARACTER VARYING, 
        'EJBCA_CERTIFICATE_RETRIEVAL'::                  CHARACTER VARYING, 
        'EJBCA_END_ENTITY_CREATION'::                    CHARACTER VARYING, 
        'PUN_CERTIFICATE_CREATION'::                     CHARACTER VARYING, 'PUN_CARRIER_RETRIEVAL' 
        ::                                               CHARACTER VARYING, 'ISSUE_EID_SIGNED':: 
                                                         CHARACTER VARYING, 'SEND_NOTIFICATION':: 
                                                         CHARACTER VARYING, 
        'CERTIFICATE_HISTORY_CREATION'::                 CHARACTER VARYING, 'PIVR_VERIFICATION':: 
                                                         CHARACTER VARYING, 'EXPORT_APPLICATION':: 
                                                         CHARACTER VARYING, 'DENIED_APPLICATION':: 
                                                         CHARACTER VARYING, 
        'E_DELIVERY_NOTIFICATION'::                      CHARACTER VARYING, 'XML_TIMESTAMP':: 
                                                         CHARACTER VARYING, 'CREATE_PAYMENT':: 
                                                         CHARACTER VARYING, 'CALCULATE_PAYMENT':: 
                                                         CHARACTER VARYING, 
        'EXISTING_CERTIFICATE_VERIFICATION'::            CHARACTER VARYING])::TEXT[])), 
        CHECK ((status)::TEXT = ANY ((ARRAY['SUBMITTED'::CHARACTER VARYING, 'PROCESSING'::CHARACTER 
        VARYING, 'PENDING_SIGNATURE'::CHARACTER VARYING, 'SIGNED'::CHARACTER VARYING, 
        'PENDING_PAYMENT'::           CHARACTER VARYING, 'PAYMENT_EXPIRED'::CHARACTER VARYING, 
        'PAID'::                      CHARACTER VARYING, 'DENIED'::CHARACTER VARYING, 'APPROVED':: 
                                      CHARACTER VARYING, 'GENERATED_CERTIFICATE'::CHARACTER VARYING 
        , 'COMPLETED'::               CHARACTER VARYING, 'CERTIFICATE_STORED'::CHARACTER VARYING]) 
        ::TEXT[])) 
);

ALTER TABLE raeicei.application
	ADD COLUMN IF NOT EXISTS service_type CHARACTER VARYING (255) NOT NULL;
	
ALTER TABLE raeicei.application_aud
	ADD COLUMN IF NOT EXISTS service_type CHARACTER VARYING (255) NOT NULL;