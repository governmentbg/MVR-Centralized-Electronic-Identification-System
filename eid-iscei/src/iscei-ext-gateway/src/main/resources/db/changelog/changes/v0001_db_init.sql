CREATE TABLE authentication_statistic (
    is_employee boolean,
    success boolean,
    create_date timestamp(6) with time zone,
    citizen_profile_id uuid,
    eidentity_id uuid,
    id uuid NOT NULL,
    x509certificate_id uuid,
    statistic_type character varying(31) NOT NULL,
    client_id character varying(255),
    level_of_assurance character varying(255),
    requester_ip_address character varying(255),
    session_id character varying(255),
    system_id character varying(255),
    system_name character varying(255),
    system_type character varying(255),
    x509certificate_issuer_dn character varying(255),
    x509certificate_sn character varying(255)
);

ALTER TABLE ONLY authentication_statistic
    ADD CONSTRAINT authentication_statistic_pkey PRIMARY KEY (id);