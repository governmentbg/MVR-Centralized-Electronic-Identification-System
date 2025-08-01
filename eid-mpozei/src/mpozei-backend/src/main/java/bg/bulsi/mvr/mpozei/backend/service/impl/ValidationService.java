package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.service.FileFormatService;
import bg.bulsi.mvr.common.service.FileFormatServiceImpl;
import bg.bulsi.mvr.common.util.CitizenIdentifierNumberValidator;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.mpozei.backend.service.RegixService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationXmlRequest;
import bg.bulsi.mvr.mpozei.contract.dto.GuardianDetails;
import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;
import bg.bulsi.mvr.mpozei.contract.dto.PersonalIdentityDocument;
import bg.bulsi.mvr.mpozei.contract.dto.xml.EidApplicationXml;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.application.StopEidApplication;
import bg.bulsi.mvr.mpozei.model.pan.EventRegistratorImpl;
import bg.bulsi.mvr.pan_client.EventRegistrator;
import bg.bulsi.mvr.pan_client.EventRegistrator.Event;
import bg.bulsi.mvr.pan_client.NotificationSender;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.stereotype.Service;

import java.util.HashSet;
import java.util.List;
import java.util.Objects;
import java.util.Set;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;

@Service
public class ValidationService {
	
	private static final String PHONE_NUMBER_REGEX = "^(?:\\+\\d{3}\\d{9}|0\\d{9})$";

	private static final String EMAIL_REGEX = "^[a-zA-Z0-9._+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";

	@Autowired
	private NotificationSender notificationSender;
	
	@Autowired
	private EventRegistrator eventRegistrator;

    @Autowired
    private RaeiceiService raeiceiService;

    @Autowired
    private FileFormatService fileFormatService;

    @Autowired
    private ApplicationMapper applicationMapper;

    public void validateCreateIssueEidApplication(AbstractApplication application) {
        Set<String> validationErrors = new HashSet<>();

        if (!validateCitizenIdentifierNumber(application.getCitizenIdentifierNumber(), application.getCitizenIdentifierType())) {
            addError(validationErrors, CITIZEN_IDENTIFIER_NOT_VALID);
        }

        if (Objects.isNull(application.getSubmissionType())) {
            addError(validationErrors, APPLICATION_SUBMISSION_TYPE_CANNOT_BE_NULL);
        }

        handleErrors(application, validationErrors);
    }

    public static boolean validateCitizenIdentifierNumber(String number, IdentifierType type) {
        return switch (type) {
            case EGN -> CitizenIdentifierNumberValidator.validateEGN(number);
            case LNCh ->  CitizenIdentifierNumberValidator.validateLNCH(number);
            default -> false;
        };
    }

    public void validateSignApplication(AbstractApplication application) {
        Set<String> validationErrors = new HashSet<>();

        validateEmail(application, validationErrors);
        validatePhoneNumber(application, validationErrors);
        validateGuardians(application, validationErrors);
        
        handleErrors(application, validationErrors);
    }

	public void validateGenerateXml(ApplicationXmlRequest xmlRequest) {
		Set<String> validationErrors = new HashSet<>();
		if (xmlRequest.getApplicationType() == ApplicationType.ISSUE_EID) {
			if (!RegixService.isBulgarian(xmlRequest.getCitizenship())) {
				addError(validationErrors, APPLICATION_TYPE_REQUIRES_DESK_FOR_FOREIGNER);
			}
		} else {
			if (xmlRequest.getCertificateId() == null) {
				addError(validationErrors, CERTIFICATE_ID_CANNOT_BE_NULL);
			}

			if (xmlRequest.getApplicationType().equals(ApplicationType.REVOKE_EID)
					&& xmlRequest.getReasonId() == null) {
				addError(validationErrors, REASON_ID_CANNOT_BE_NULL);
			}
		}

		handleErrors(validationErrors);
	}
    
    public void validateIssueEidOnlineApplication(AbstractApplication application) {
        Set<String> validationErrors = new HashSet<>();

        if (!validateCitizenIdentifierNumber(application.getCitizenIdentifierNumber(), application.getCitizenIdentifierType())) {
            addError(validationErrors, CITIZEN_IDENTIFIER_NOT_VALID);
        }

        if (Objects.isNull(application.getSubmissionType())) {
            addError(validationErrors, APPLICATION_SUBMISSION_TYPE_CANNOT_BE_NULL);
        }

        validateEmail(application, validationErrors);
        validatePhoneNumber(application, validationErrors);

        this.validateCitizenSecondAndLastName(application, validationErrors);
        this.validateCitizenSecondAndLastNameLatin(application, validationErrors);

        handleErrors(application, validationErrors);
    }

    public void validateIssueEidOnlineEidApplication(AbstractApplication application) {
        Set<String> validationErrors = new HashSet<>();

        if (!validateCitizenIdentifierNumber(application.getCitizenIdentifierNumber(), application.getCitizenIdentifierType())) {
            addError(validationErrors, CITIZEN_IDENTIFIER_NOT_VALID);
        }

        if (Objects.isNull(application.getSubmissionType())) {
            addError(validationErrors, APPLICATION_SUBMISSION_TYPE_CANNOT_BE_NULL);
        }

        this.validateCitizenSecondAndLastName(application, validationErrors);
        this.validateCitizenSecondAndLastNameLatin(application, validationErrors);
        
        handleErrors(application, validationErrors);
    }

    public void validateStopEidApplication(StopEidApplication application) {
        Set<String> validationErrors = new HashSet<>();
        
        this.validateCitizenSecondAndLastName(application, validationErrors);
        this.validateCitizenSecondAndLastNameLatin(application, validationErrors);
        
        // validate certificateId
        if (Objects.isNull(application.getParams().getCertificateId())) {
            addError(validationErrors, CERTIFICATE_ID_CANNOT_BE_NULL);
        }

        if (!validateCitizenIdentifierNumber(application.getCitizenIdentifierNumber(), application.getCitizenIdentifierType())) {
            addError(validationErrors, CITIZEN_IDENTIFIER_NOT_VALID);
        }

        // validate reason
        if (Objects.isNull(application.getReason())) {
            addError(validationErrors, REASON_CANNOT_BE_NULL);
        }
        
		// validate application submission type
		if (Objects.isNull(application.getSubmissionType())) {
			addError(validationErrors, APPLICATION_SUBMISSION_TYPE_CANNOT_BE_NULL);
		}

        this.validatePersonalIdDocument(application.getTemporaryData().getPersonalIdentityDocument(), validationErrors);

        handleErrors(application, validationErrors);
    }
    
    
    public void validateSignStopEidApplication(AbstractApplication application) {
        Set<String> validationErrors = new HashSet<>();

    	validateGuardians(application, validationErrors);
        validateEmail(application, validationErrors);
        validatePhoneNumber(application, validationErrors);

        handleErrors(application, validationErrors);
    }

    public void validateSignResumeEidFromDeskApplication(AbstractApplication application) {
        Set<String> validationErrors = new HashSet<>();

        validateGuardians(application, validationErrors);
        validateEmail(application, validationErrors);
        validatePhoneNumber(application, validationErrors);

        handleErrors(application, validationErrors);
    }
    
    
    public void validateResumeEidApplication(AbstractApplication application) {
        Set<String> validationErrors = new HashSet<>();

        this.validateCitizenSecondAndLastName(application, validationErrors);
        this.validateCitizenSecondAndLastNameLatin(application, validationErrors);
        
        // validate certificateId
        if (Objects.isNull(application.getParams().getCertificateId())) {
            addError(validationErrors, CERTIFICATE_ID_CANNOT_BE_NULL);
        }

        if (!validateCitizenIdentifierNumber(application.getCitizenIdentifierNumber(), application.getCitizenIdentifierType())) {
            addError(validationErrors, CITIZEN_IDENTIFIER_NOT_VALID);
        }

        if (Objects.isNull(application.getSubmissionType())) {
            addError(validationErrors, APPLICATION_SUBMISSION_TYPE_CANNOT_BE_NULL);
        }

        this.validatePersonalIdDocument(application.getTemporaryData().getPersonalIdentityDocument(), validationErrors);

        handleErrors(application, validationErrors);
    }

    public void validateRevokeEidFromDeskApplication(AbstractApplication application) {
        Set<String> validationErrors = new HashSet<>();

        this.validateCitizenSecondAndLastName(application, validationErrors);
        this.validateCitizenSecondAndLastNameLatin(application, validationErrors);
        
        // validate certificateId
        if (Objects.isNull(application.getParams().getCertificateId())) {
            addError(validationErrors, CERTIFICATE_ID_CANNOT_BE_NULL);
        }

        if (!validateCitizenIdentifierNumber(application.getCitizenIdentifierNumber(), application.getCitizenIdentifierType())) {
            addError(validationErrors, CITIZEN_IDENTIFIER_NUMBER_NOT_VALID);
        }

        // validate citizenship
        if (Objects.isNull(application.getCitizenship())) {
            addError(validationErrors, CITIZENSHIP_CANNOT_BE_NULL);
        }

        // validate reason
        if (Objects.isNull(application.getReason())) {
            addError(validationErrors, REASON_CANNOT_BE_NULL);
        }

        // validate application submission type
        if (Objects.isNull(application.getSubmissionType())) {
            addError(validationErrors, APPLICATION_SUBMISSION_TYPE_CANNOT_BE_NULL);
        }

        this.validatePersonalIdDocument(application.getTemporaryData().getPersonalIdentityDocument(), validationErrors);

        handleErrors(application, validationErrors);
    }

    public void validateSignRevokeEidFromDeskApplication(AbstractApplication application) {
        Set<String> validationErrors = new HashSet<>();

        validateGuardians(application, validationErrors);
        validateEmail(application, validationErrors);
        validatePhoneNumber(application, validationErrors);

        handleErrors(application, validationErrors);
    }

	public void validateCitizenIdentifierAndType(String number, String type) {
        Set<String> validationErrors = new HashSet<>();

        if (!validateCitizenIdentifierNumber(number, IdentifierType.valueOf(type))) {
            addError(validationErrors, CITIZEN_IDENTIFIER_NUMBER_NOT_VALID);
        }

        handleErrors(validationErrors);
    }
    
	public void validateGuardiansOnlineApplication(AbstractApplication application) {
		Set<String> validationErrors = new HashSet<>();
		
		validateGuardians(application, validationErrors);

		handleErrors(application, validationErrors);
	}
	
	public void validateGuardians(AbstractApplication application, Set<String> validationErrors) {
        EidApplicationXml applicationXml = fileFormatService.createObjectFromXmlString(application.getApplicationXml(), EidApplicationXml.class);

        List<GuardianDetails> guardianDetails;
        if (Objects.nonNull(applicationXml.getGuardians()) && !applicationXml.getGuardians().isEmpty()) {
            guardianDetails = applicationMapper.map(applicationXml.getGuardians());
        } else {
            guardianDetails = application.getTemporaryData().getGuardians();
        }
		//Check is previously done, so skip for this specific case
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        if (DeviceType.MOBILE.equals(device.getType())
        		&& Boolean.TRUE.equals(application.getParams().getRequireGuardians())
        		&& (guardianDetails == null || guardianDetails.isEmpty())) {
        	return;
        } 
        
        //EID-4505
		if (application.getReason() != null 
				&& ("REVOKED_BY_ADMINISTRATOR".equals(application.getReason().getName())
				 || "STOPPED_BY_ADMINISTRATOR".equals(application.getReason().getName()))) {
			return;
		}
        
		if (Boolean.TRUE.equals(application.getParams().getRequireGuardians()) && (guardianDetails == null || guardianDetails.isEmpty())) {
			addError(validationErrors, GUARDIAN_DETAILS_CANNOT_BE_NULL);
		} else if (Boolean.TRUE.equals(application.getParams().getRequireGuardians()) && (guardianDetails != null && !guardianDetails.isEmpty())){
			// check if the guardians data is correct
			for (GuardianDetails guardian : guardianDetails) {
				if(StringUtils.isBlank(guardian.getFirstName())) {
					addError(validationErrors, GUARDIAN_FIRST_NAME_CANNOT_BE_BLANK);
				}
				
				if(StringUtils.isBlank(guardian.getSecondName())) {
					addError(validationErrors, GUARDIAN_SECOND_NAME_CANNOT_BE_BLANK);
				}
				
				if(StringUtils.isBlank(guardian.getLastName())) {
					addError(validationErrors, GUARDIAN_LAST_NAME_CANNOT_BE_BLANK);
				}
				
				//this.validateGuardianSecondAndLastName(guardian.getSecondName(), guardian.getLastName(), validationErrors);
				
				if(StringUtils.isBlank(guardian.getCitizenIdentifierNumber())) {
					addError(validationErrors, GUARDIAN_CITIZEN_IDENTIFIER_NUMBER_CANNOT_BE_BLANK);
				}
				
				if(Objects.isNull(guardian.getCitizenIdentifierType())) {
					addError(validationErrors, GUARDIAN_CITIZEN_IDENTIFIER_TYPE_CANNOT_BE_NULL);
				}
				
				PersonalIdentityDocument personalIdentityDocument = guardian.getPersonalIdentityDocument();
				if(Objects.isNull(personalIdentityDocument)) {
					addError(validationErrors, GUARDIAN_IDENTITY_DOCUMENT_CANNOT_BE_NULL);
				} else {
					if(Objects.isNull(personalIdentityDocument.getIdentityIssueDate())) {
						addError(validationErrors, GUARDIAN_IDENTITY_ISSUE_DATE_CANNOT_BE_NULL);
					}
					
					if(Objects.isNull(personalIdentityDocument.getIdentityValidityToDate())) {
						addError(validationErrors, GUARDIAN_IDENTITY_VALIDITY_DATE_CANNOT_BE_NULL);
					}
					
					if(StringUtils.isBlank(personalIdentityDocument.getIdentityType())) {
						addError(validationErrors, GUARDIAN_IDENTITY_TYPE_CANNOT_BE_BLANK);
					}
					
					if(StringUtils.isBlank(personalIdentityDocument.getIdentityNumber())) {
						addError(validationErrors, GUARDIAN_IDENTITY_NUMBER_CANNOT_BE_BLANK);
					}
				}
			}
		}
	}
    
	private void handleErrors(AbstractApplication application, Set<String> validationErrors) {
        if (!validationErrors.isEmpty()) {
        	if(application.getEidentityId() != null) {
        		Event event = eventRegistrator.getEvent(EventRegistratorImpl.MPOZEI_E_DENIED_DUE_TO_INVALID_CHECK_EID);
            	this.notificationSender.send(event.code(), application.getEidentityId());
        	}

            throw new ValidationMVRException("Request is not valid", VALIDATION_ERROR, new HashSet<>(validationErrors));
        }
	}

    private void handleErrors(Set<String> validationErrors) {
        if (!validationErrors.isEmpty()) {
            throw new ValidationMVRException("Request is not valid", VALIDATION_ERROR, new HashSet<>(validationErrors));
        }
    }
	
	private void validatePersonalIdDocument(PersonalIdentityDocument personalIdentityDocument, Set<String> validationErrors) {
		if(Objects.isNull(personalIdentityDocument)) {
			addError(validationErrors, GUARDIAN_IDENTITY_DOCUMENT_CANNOT_BE_NULL);
		} else {
			if(Objects.isNull(personalIdentityDocument.getIdentityIssueDate())) {
				addError(validationErrors, GUARDIAN_IDENTITY_ISSUE_DATE_CANNOT_BE_NULL);
			}

			if(Objects.isNull(personalIdentityDocument.getIdentityValidityToDate())) {
				addError(validationErrors, GUARDIAN_IDENTITY_VALIDITY_DATE_CANNOT_BE_NULL);
			}

			if(StringUtils.isBlank(personalIdentityDocument.getIdentityType())) {
				addError(validationErrors, GUARDIAN_IDENTITY_TYPE_CANNOT_BE_BLANK);
			}

			if(StringUtils.isBlank(personalIdentityDocument.getIdentityNumber())) {
				addError(validationErrors, GUARDIAN_IDENTITY_NUMBER_CANNOT_BE_BLANK);
			}
		}
	}

//    public void validateExportEidApplication(AbstractApplication application) {
//        Map<String, List<String>> validationErrors = new HashMap<>();
//        
//        ApplicationExport applicationExport = application.getTemporaryData().getApplicationExportRequest();
//        
//        // validate personalIdIssuer
//        if (Objects.isNull(applicationExport.getPersonalIdIssuer())) {
//            addError(validationErrors, "personalIdIssuer", "Personal Id Issuer cannot be null");
//        }
//
//        // validate personalIdIssueDate
//        if (Objects.isNull(applicationExport.getPersonalIdIssueDate())) {
//            addError(validationErrors, "personalIdIssueDate", "Personal Id Issue Date cannot be null");
//        }
//        
//        // validate personalIdValidityToDate
//        if (Objects.isNull(applicationExport.getPersonalIdValidityToDate())) {
//            addError(validationErrors, "personalIdValidityToDate", "Personal Id Validity To Date cannot be null");
//        }
//
//        // validate personalIdNumber
//        if (Objects.isNull(applicationExport.getPersonalIdNumber())) {
//            addError(validationErrors, "personalIdNumber", "Personal Identifier Number cannot be null");
//        } 
//
//        if (!validationErrors.isEmpty()) {
//            throw new ValidationMVRException("Request is not valid", new HashMap<>(validationErrors));
//        }
//    }

    private void addError(Set<String> errors, ErrorCode error) {
        if (errors == null) {
            errors = new HashSet<>();
        }
        errors.add(error.name());
    }

    private void validateEmail(AbstractApplication application, Set<String> validationErrors) {
        if (Objects.nonNull(application.getParams().getEmail())) {
            if (application.getParams().getEmail().isBlank()) {
                application.getParams().setEmail(null);
            } else {
                if (!application.getParams().getEmail().matches(EMAIL_REGEX)) {
                    addError(validationErrors, EMAIL_NOT_VALID);
                }
            }
        }
    }

    private void validatePhoneNumber(AbstractApplication application, Set<String> validationErrors) {
        if (Objects.nonNull(application.getParams().getPhoneNumber())) {
            if (application.getParams().getPhoneNumber().isBlank()) {
                application.getParams().setPhoneNumber(null);
            } else {
                if (!application.getParams().getPhoneNumber().matches(PHONE_NUMBER_REGEX)) {
                    addError(validationErrors, PHONE_NUMBER_NOT_VALID);
                }
            }
        }
    }
    
    private void validateGuardianSecondAndLastName(String secondName, String lastName, Set<String> validationErrors) {
        if (StringUtils.isBlank(secondName) && StringUtils.isBlank(lastName)) {
        	addError(validationErrors, GUARDIAN_SECOND_OR_LAST_NAME_CANNOT_BE_BLANK);
        }
    }
    
    private void validateCitizenSecondAndLastName(AbstractApplication application, Set<String> validationErrors) {
        if (application.getSecondName() == null && application.getLastName() == null) {
        	addError(validationErrors, SECOND_OR_LAST_NAME_REQUIRED);
        }
    }
    
    private void validateCitizenSecondAndLastNameLatin(AbstractApplication application, Set<String> validationErrors) {
        if (application.getSecondNameLatin() == null && application.getLastNameLatin() == null) {
        	addError(validationErrors, SECOND_OR_LAST_NAME_LATIN_REQUIRED);
        }
    }
}
