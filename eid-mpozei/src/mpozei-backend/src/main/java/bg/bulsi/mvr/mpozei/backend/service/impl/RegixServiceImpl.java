package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.mpozei.backend.client.PivrClient;
import bg.bulsi.mvr.mpozei.backend.dto.RegixIdentityInfoDTO;
import bg.bulsi.mvr.mpozei.backend.service.RegixService;
import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;
import bg.bulsi.mvr.mpozei.model.pivr.ForeignCitizenType;
import bg.bulsi.mvr.mpozei.model.pivr.ForeignIdentityInfoResponseType;
import bg.bulsi.mvr.mpozei.model.pivr.IdentityDocument;
import bg.bulsi.mvr.mpozei.model.pivr.PersonalIdentityInfoResponseType;
import bg.bulsi.mvr.mpozei.model.pivr.common.PersonNames;
import bg.bulsi.mvr.mpozei.model.pivr.common.ReturnInformation;
import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializationFeature;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import lombok.extern.slf4j.Slf4j;

import org.apache.commons.collections4.CollectionUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.converter.json.Jackson2ObjectMapperBuilder;
import org.springframework.stereotype.Component;

import static bg.bulsi.mvr.common.exception.ErrorCode.COULD_NOT_VERIFY_CITIZEN_DATA;
import static bg.bulsi.mvr.common.exception.ErrorCode.REGIX_EXCEPTION;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertEquals;

@Slf4j
@Component
public class RegixServiceImpl implements RegixService {
    private PivrClient pivrClient;
    private ObjectMapper objectMapper;

    @Autowired
    public RegixServiceImpl(PivrClient pivrClient) {
        this.pivrClient = pivrClient;
        this.objectMapper = Jackson2ObjectMapperBuilder.json()
                .modules(new JavaTimeModule())
                .featuresToDisable(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS, DeserializationFeature.ADJUST_DATES_TO_CONTEXT_TIME_ZONE)
                .build();

    }

    @Override
    public RegixIdentityInfoDTO getIdentityInfoFromRegix(String citizenIdentifierNumber, IdentifierType type, String personalIdNumber) {
        RegixIdentityInfoDTO dto = new RegixIdentityInfoDTO();
        if(type == IdentifierType.EGN) {
            PersonalIdentityInfoResponseType responseType =
                    this.objectMapper.convertValue(
                            (pivrClient.getPersonalIdentityV2(citizenIdentifierNumber, personalIdNumber).getResponse().get("PersonalIdentityInfoResponse")),
                            PersonalIdentityInfoResponseType.class);

            validateReturnInformation(responseType.getReturnInformations());

            dto.setPersonalIdIssueDate(responseType.getIssueDate());
            dto.setPersonalIdValidityToDate(responseType.getValidDate());
            dto.setPersonalIdIssuer(responseType.getIssuerName());
            dto.setPersonalIdIssuerLatin(responseType.getIssuerNameLatin());
            dto.setPersonalIdNumber(responseType.getIdentityDocumentNumber());
            dto.setPersonalIdDocumentType(responseType.getDocumentType());
            dto.setPersonalIdDocumentTypeLatin(responseType.getDocumentTypeLatin());
            
            if(CollectionUtils.isNotEmpty(responseType.getNationalityList()) 
            		&& responseType.getNationalityList().stream().anyMatch(n -> 
            		BULGARIA_COUNTRY_CODE.equals(n.getNationalityCode()) || BULGARIA_COUNTRY_CODE_LATIN.equals(n.getNationalityCode()))) {
               
                dto.setBirthDate(responseType.getBirthDate());
                dto.setFirstNameLatin(responseType.getPersonNames().getFirstNameLatin());
                dto.setSecondNameLatin(responseType.getPersonNames().getSurnameLatin());
                dto.setLastNameLatin(responseType.getPersonNames().getLastNameLatin());
                dto.setNationalityList(responseType.getNationalityList());
            } else {
            	//Some Foreign citizens can have EGN
            	ForeignCitizenType dataForeignCitizen = responseType.getDataForeignCitizen();
            	PersonNames personNames = dataForeignCitizen.getNames();
            	
                dto.setBirthDate(dataForeignCitizen.getBirthDate());
                dto.setFirstNameLatin(personNames.getFirstNameLatin());
                dto.setLastNameLatin(personNames.getLastNameLatin());
                dto.setNationalityList(dataForeignCitizen.getNationalityList());
            }
        } else if(type == IdentifierType.LNCh){
            ForeignIdentityInfoResponseType responseType =
                    this.objectMapper.convertValue(
                            (pivrClient.getForeignIdentityV2(IdentifierType.LNCh, citizenIdentifierNumber).getResponse().get("ForeignIdentityInfoResponse")),
                            ForeignIdentityInfoResponseType.class);

            validateReturnInformation(responseType.getReturnInformations());

            IdentityDocument identityDocument = responseType.getIdentityDocument();

            dto.setPersonalIdIssueDate(identityDocument.getIssueDate());
            dto.setPersonalIdValidityToDate(identityDocument.getValidDate());
            dto.setPersonalIdIssuer(identityDocument.getIssuerName());
            dto.setPersonalIdIssuerLatin(identityDocument.getIssuerNameLatin());
            dto.setPersonalIdNumber(identityDocument.getIdentityDocumentNumber());
            dto.setPersonalIdDocumentType(identityDocument.getDocumentType());
            dto.setPersonalIdDocumentTypeLatin(identityDocument.getDocumentTypeLatin());

            dto.setBirthDate(responseType.getBirthDateParsed().orElse(null));
            dto.setFirstNameLatin(responseType.getPersonNames().getFirstNameLatin());
            dto.setSecondNameLatin(responseType.getPersonNames().getSurnameLatin());
            dto.setLastNameLatin(responseType.getPersonNames().getLastNameLatin());
            dto.setNationalityList(responseType.getNationalityList());
        } else {
            throw new ValidationMVRException(COULD_NOT_VERIFY_CITIZEN_DATA);
        }
        return dto;
    }

    private void validateReturnInformation(ReturnInformation returnInformation) {
    	//TODO: Check what does Regix returns as error 
        assertEquals(returnInformation.getReturnCode(), "0000", REGIX_EXCEPTION);
        log.error(returnInformation.getInfo());
    }
}
