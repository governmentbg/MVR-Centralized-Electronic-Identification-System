--
-- PostgreSQL database dump
--

-- Dumped from database version 14.1
-- Dumped by pg_dump version 16.2

--
-- Name: devices; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.devices (
    id uuid NOT NULL,
    create_date timestamp(6) without time zone,
    created_by character varying(255),
    last_update timestamp(6) without time zone,
    updated_by character varying(255),
    version bigint,
    description character varying(255),
    name character varying(255) NOT NULL,
    type character varying(255) NOT NULL
);


--
-- Name: devices_aud; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.devices_aud (
    id uuid NOT NULL,
    rev integer NOT NULL,
    revtype smallint,
    description character varying(255),
    name character varying(255),
    type character varying(255)
);


--
-- Name: discounts; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.discounts (
    id uuid NOT NULL,
    create_date timestamp(6) without time zone,
    created_by character varying(255),
    last_update timestamp(6) without time zone,
    updated_by character varying(255),
    version bigint,
    age_from integer NOT NULL,
    age_until integer NOT NULL,
    disability boolean,
    start_date date NOT NULL,
    value double precision NOT NULL,
    eid_manager_id uuid NOT NULL
);


--
-- Name: discounts_aud; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.discounts_aud (
    id uuid NOT NULL,
    rev integer NOT NULL,
    revtype smallint,
    age_from integer,
    age_until integer,
    disability boolean,
    start_date date,
    value double precision,
    eid_manager_id uuid
);


--
-- Name: eid_administrator_device; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.eid_administrator_device (
    eid_administrator_id uuid NOT NULL,
    device_id uuid NOT NULL
);


--
-- Name: eid_administrator_device_aud; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.eid_administrator_device_aud (
    rev integer NOT NULL,
    eid_administrator_id uuid NOT NULL,
    device_id uuid NOT NULL,
    revtype smallint
);


--
-- Name: eid_manager; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.eid_manager (
    service_type character varying(31) NOT NULL,
    id uuid NOT NULL,
    create_date timestamp(6) without time zone,
    created_by character varying(255),
    last_update timestamp(6) without time zone,
    updated_by character varying(255),
    version bigint,
    address character varying(255),
    contact character varying(255),
    eik_number character varying(255) NOT NULL,
    email character varying(255),
    home_page character varying(255),
    is_active boolean,
    logo character varying(255),
    name character varying(255) NOT NULL,
    name_latin character varying(255) NOT NULL,
    client_id character varying(255),
    client_secret character varying(255)
);


--
-- Name: eid_manager_aud; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.eid_manager_aud (
    id uuid NOT NULL,
    rev integer NOT NULL,
    service_type character varying(31) NOT NULL,
    revtype smallint,
    address character varying(255),
    contact character varying(255),
    eik_number character varying(255),
    email character varying(255),
    home_page character varying(255),
    is_active boolean,
    logo character varying(255),
    name character varying(255),
    name_latin character varying(255),
    client_id character varying(255),
    client_secret character varying(255)
);


--
-- Name: eid_manager_front_office; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.eid_manager_front_office (
    id uuid NOT NULL,
    create_date timestamp(6) without time zone,
    created_by character varying(255),
    last_update timestamp(6) without time zone,
    updated_by character varying(255),
    version bigint,
    contact character varying(255) NOT NULL,
    email character varying(255),
    is_active boolean,
    latitude character varying(255),
    location character varying(255) NOT NULL,
    longitude character varying(255),
    name character varying(255) NOT NULL,
    region character varying(255),
    working_hours character varying(255),
    eid_manager_id uuid NOT NULL,
    CONSTRAINT eid_manager_front_office_region_check CHECK (((region)::text = ANY ((ARRAY['BLAGOEVGRAD'::character varying, 'BURGAS'::character varying, 'VARNA'::character varying, 'VELIKO_TARNOVO'::character varying, 'VIDIN'::character varying, 'VRATSA'::character varying, 'GABROVO'::character varying, 'DOBRICH'::character varying, 'KARDZHALI'::character varying, 'KYUSTENDIL'::character varying, 'LOVECH'::character varying, 'MONTANA'::character varying, 'PAZARDZHIK'::character varying, 'PERNIK'::character varying, 'PLEVEN'::character varying, 'PLOVDIV'::character varying, 'RAZGRAD'::character varying, 'RUSE'::character varying, 'SILISTRA'::character varying, 'SLIVEN'::character varying, 'SMOLYAN'::character varying, 'SOFIA'::character varying, 'SOFIA_CITY'::character varying, 'STARA_ZAGORA'::character varying, 'TARGOVISHTE'::character varying, 'HASKOVO'::character varying, 'SHUMEN'::character varying, 'YAMBOL'::character varying, 'MVR'::character varying])::text[])))
);


--
-- Name: eid_manager_front_office_aud; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.eid_manager_front_office_aud (
    id uuid NOT NULL,
    rev integer NOT NULL,
    revtype smallint,
    contact character varying(255),
    email character varying(255),
    is_active boolean,
    latitude character varying(255),
    location character varying(255),
    longitude character varying(255),
    name character varying(255),
    region character varying(255),
    working_hours character varying(255),
    eid_manager_id uuid,
    CONSTRAINT eid_manager_front_office_aud_region_check CHECK (((region)::text = ANY ((ARRAY['BLAGOEVGRAD'::character varying, 'BURGAS'::character varying, 'VARNA'::character varying, 'VELIKO_TARNOVO'::character varying, 'VIDIN'::character varying, 'VRATSA'::character varying, 'GABROVO'::character varying, 'DOBRICH'::character varying, 'KARDZHALI'::character varying, 'KYUSTENDIL'::character varying, 'LOVECH'::character varying, 'MONTANA'::character varying, 'PAZARDZHIK'::character varying, 'PERNIK'::character varying, 'PLEVEN'::character varying, 'PLOVDIV'::character varying, 'RAZGRAD'::character varying, 'RUSE'::character varying, 'SILISTRA'::character varying, 'SLIVEN'::character varying, 'SMOLYAN'::character varying, 'SOFIA'::character varying, 'SOFIA_CITY'::character varying, 'STARA_ZAGORA'::character varying, 'TARGOVISHTE'::character varying, 'HASKOVO'::character varying, 'SHUMEN'::character varying, 'YAMBOL'::character varying, 'MVR'::character varying])::text[])))
);


--
-- Name: eid_manager_provided_service; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.eid_manager_provided_service (
    eid_manager_id uuid NOT NULL,
    provided_service_id uuid NOT NULL
);


--
-- Name: eid_manager_provided_service_aud; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.eid_manager_provided_service_aud (
    rev integer NOT NULL,
    eid_manager_id uuid NOT NULL,
    provided_service_id uuid NOT NULL,
    revtype smallint
);


--
-- Name: provided_service; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.provided_service (
    service_type character varying(31) NOT NULL,
    id uuid NOT NULL,
    create_date timestamp(6) without time zone,
    created_by character varying(255),
    last_update timestamp(6) without time zone,
    updated_by character varying(255),
    version bigint,
    name character varying(255) NOT NULL,
    name_latin character varying(255) NOT NULL,
    application_type character varying(255)
);


--
-- Name: provided_service_aud; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.provided_service_aud (
    id uuid NOT NULL,
    rev integer NOT NULL,
    service_type character varying(31) NOT NULL,
    revtype smallint,
    name character varying(255),
    name_latin character varying(255),
    application_type character varying(255)
);


--
-- Name: revinfo; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.revinfo (
    rev integer NOT NULL,
    revtstmp bigint
);


--
-- Name: tariffs; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.tariffs (
    tariff_type character varying(31) NOT NULL,
    id uuid NOT NULL,
    create_date timestamp(6) without time zone,
    created_by character varying(255),
    last_update timestamp(6) without time zone,
    updated_by character varying(255),
    version bigint,
    currency character varying(255),
    price numeric(38,2) NOT NULL,
    start_date date NOT NULL,
    eid_manager_id uuid NOT NULL,
    device_id uuid,
    provided_service_id uuid,
    CONSTRAINT tariffs_currency_check CHECK (((currency)::text = ANY ((ARRAY['BGN'::character varying, 'EUR'::character varying])::text[])))
);


--
-- Name: tariffs_aud; Type: TABLE; Schema: raeicei; Owner: -
--

CREATE TABLE raeicei.tariffs_aud (
    id uuid NOT NULL,
    rev integer NOT NULL,
    tariff_type character varying(31) NOT NULL,
    revtype smallint,
    currency character varying(255),
    price numeric(38,2),
    start_date date,
    eid_manager_id uuid,
    provided_service_id uuid,
    device_id uuid,
    CONSTRAINT tariffs_aud_currency_check CHECK (((currency)::text = ANY ((ARRAY['BGN'::character varying, 'EUR'::character varying])::text[])))
);


--
-- Name: devices_aud devices_aud_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.devices_aud
    ADD CONSTRAINT devices_aud_pkey PRIMARY KEY (rev, id);


--
-- Name: devices devices_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.devices
    ADD CONSTRAINT devices_pkey PRIMARY KEY (id);


--
-- Name: discounts_aud discounts_aud_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.discounts_aud
    ADD CONSTRAINT discounts_aud_pkey PRIMARY KEY (rev, id);


--
-- Name: discounts discounts_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.discounts
    ADD CONSTRAINT discounts_pkey PRIMARY KEY (id);


--
-- Name: eid_administrator_device_aud eid_administrator_device_aud_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_administrator_device_aud
    ADD CONSTRAINT eid_administrator_device_aud_pkey PRIMARY KEY (eid_administrator_id, rev, device_id);


--
-- Name: eid_manager_aud eid_manager_aud_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager_aud
    ADD CONSTRAINT eid_manager_aud_pkey PRIMARY KEY (rev, id);


--
-- Name: eid_manager_front_office_aud eid_manager_front_office_aud_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager_front_office_aud
    ADD CONSTRAINT eid_manager_front_office_aud_pkey PRIMARY KEY (rev, id);


--
-- Name: eid_manager_front_office eid_manager_front_office_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager_front_office
    ADD CONSTRAINT eid_manager_front_office_pkey PRIMARY KEY (id);


--
-- Name: eid_manager eid_manager_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager
    ADD CONSTRAINT eid_manager_pkey PRIMARY KEY (id);


--
-- Name: eid_manager_provided_service_aud eid_manager_provided_service_aud_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager_provided_service_aud
    ADD CONSTRAINT eid_manager_provided_service_aud_pkey PRIMARY KEY (eid_manager_id, rev, provided_service_id);


--
-- Name: provided_service_aud provided_service_aud_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.provided_service_aud
    ADD CONSTRAINT provided_service_aud_pkey PRIMARY KEY (rev, id);


--
-- Name: provided_service provided_service_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.provided_service
    ADD CONSTRAINT provided_service_pkey PRIMARY KEY (id);


--
-- Name: revinfo revinfo_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.revinfo
    ADD CONSTRAINT revinfo_pkey PRIMARY KEY (rev);


--
-- Name: tariffs_aud tariffs_aud_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.tariffs_aud
    ADD CONSTRAINT tariffs_aud_pkey PRIMARY KEY (rev, id);


--
-- Name: tariffs tariffs_pkey; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.tariffs
    ADD CONSTRAINT tariffs_pkey PRIMARY KEY (id);


--
-- Name: provided_service uk_i2vrps8n8jpu8gv4relgx7mbo; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.provided_service
    ADD CONSTRAINT uk_i2vrps8n8jpu8gv4relgx7mbo UNIQUE (name_latin);


--
-- Name: eid_manager uk_m7vvtost7dyp0wamvgppo3ite; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager
    ADD CONSTRAINT uk_m7vvtost7dyp0wamvgppo3ite UNIQUE (name);


--
-- Name: eid_manager uk_nemtaf91d1rsrmugplv915joc; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager
    ADD CONSTRAINT uk_nemtaf91d1rsrmugplv915joc UNIQUE (name_latin);


--
-- Name: provided_service uk_rnb8n06t130uugw4979k9srln; Type: CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.provided_service
    ADD CONSTRAINT uk_rnb8n06t130uugw4979k9srln UNIQUE (name);


--
-- Name: devices_aud fk334l0xy7gy0qjjfr5pe1wwxie; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.devices_aud
    ADD CONSTRAINT fk334l0xy7gy0qjjfr5pe1wwxie FOREIGN KEY (rev) REFERENCES raeicei.revinfo(rev);


--
-- Name: eid_administrator_device_aud fk4obkq2b89x0uo9puu5qmrrxxd; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_administrator_device_aud
    ADD CONSTRAINT fk4obkq2b89x0uo9puu5qmrrxxd FOREIGN KEY (rev) REFERENCES raeicei.revinfo(rev);


--
-- Name: tariffs_aud fk5jarao91dpmdsml4dvdnywne4; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.tariffs_aud
    ADD CONSTRAINT fk5jarao91dpmdsml4dvdnywne4 FOREIGN KEY (rev) REFERENCES raeicei.revinfo(rev);


--
-- Name: eid_administrator_device fk705kfsxhc8lec8sg4lskleqc0; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_administrator_device
    ADD CONSTRAINT fk705kfsxhc8lec8sg4lskleqc0 FOREIGN KEY (device_id) REFERENCES raeicei.devices(id);


--
-- Name: tariffs fk7j0riu91qquk6asc0o2911sde; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.tariffs
    ADD CONSTRAINT fk7j0riu91qquk6asc0o2911sde FOREIGN KEY (eid_manager_id) REFERENCES raeicei.eid_manager(id);


--
-- Name: eid_manager_provided_service_aud fkaw1hxh196lu41aukfp7qx0cmg; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager_provided_service_aud
    ADD CONSTRAINT fkaw1hxh196lu41aukfp7qx0cmg FOREIGN KEY (rev) REFERENCES raeicei.revinfo(rev);


--
-- Name: eid_manager_front_office fkcbfkcf8wkgpmqd6mkd57kk0eb; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager_front_office
    ADD CONSTRAINT fkcbfkcf8wkgpmqd6mkd57kk0eb FOREIGN KEY (eid_manager_id) REFERENCES raeicei.eid_manager(id);


--
-- Name: eid_administrator_device fkdhun47ycss6a9turvs84emu7d; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_administrator_device
    ADD CONSTRAINT fkdhun47ycss6a9turvs84emu7d FOREIGN KEY (eid_administrator_id) REFERENCES raeicei.eid_manager(id);


--
-- Name: tariffs fkelbqj67nrjhcqafng6e59fem0; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.tariffs
    ADD CONSTRAINT fkelbqj67nrjhcqafng6e59fem0 FOREIGN KEY (provided_service_id) REFERENCES raeicei.provided_service(id);


--
-- Name: discounts fkglg2mt9f7m5oxlrjh1noceq2w; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.discounts
    ADD CONSTRAINT fkglg2mt9f7m5oxlrjh1noceq2w FOREIGN KEY (eid_manager_id) REFERENCES raeicei.eid_manager(id);


--
-- Name: provided_service_aud fki974q2iipojnnty6u495l75rn; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.provided_service_aud
    ADD CONSTRAINT fki974q2iipojnnty6u495l75rn FOREIGN KEY (rev) REFERENCES raeicei.revinfo(rev);


--
-- Name: eid_manager_aud fkk51ypibeptgekaykb10vysoxw; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager_aud
    ADD CONSTRAINT fkk51ypibeptgekaykb10vysoxw FOREIGN KEY (rev) REFERENCES raeicei.revinfo(rev);


--
-- Name: eid_manager_provided_service fkknfbw3mphwghcgusust8n45ni; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager_provided_service
    ADD CONSTRAINT fkknfbw3mphwghcgusust8n45ni FOREIGN KEY (eid_manager_id) REFERENCES raeicei.eid_manager(id);


--
-- Name: discounts_aud fkn2d7xmo6yoj2m4nso3eta8cdu; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.discounts_aud
    ADD CONSTRAINT fkn2d7xmo6yoj2m4nso3eta8cdu FOREIGN KEY (rev) REFERENCES raeicei.revinfo(rev);


--
-- Name: eid_manager_front_office_aud fkp5fct0pd5ln87dlgtssfoha68; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager_front_office_aud
    ADD CONSTRAINT fkp5fct0pd5ln87dlgtssfoha68 FOREIGN KEY (rev) REFERENCES raeicei.revinfo(rev);


--
-- Name: eid_manager_provided_service fkqnm67shf50dhugnvlbp60vw98; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.eid_manager_provided_service
    ADD CONSTRAINT fkqnm67shf50dhugnvlbp60vw98 FOREIGN KEY (provided_service_id) REFERENCES raeicei.provided_service(id);


--
-- Name: tariffs fks5m7qv69i6od1spj2noopjps5; Type: FK CONSTRAINT; Schema: raeicei; Owner: -
--

ALTER TABLE ONLY raeicei.tariffs
    ADD CONSTRAINT fks5m7qv69i6od1spj2noopjps5 FOREIGN KEY (device_id) REFERENCES raeicei.devices(id);


--
-- PostgreSQL database dump complete
--

