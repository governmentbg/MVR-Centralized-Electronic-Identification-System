//package bg.bulsi.mvr.mpozei.backend.mapper;
//
//import java.util.Collections;
//import java.util.EnumMap;
//import java.util.Map;
//
//import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
//
///**
// * Enum used to map {@link ApplicationType} to the name of pdf form that they should used.
// */
//public enum ApplicationTypeToFormMapper {
//	
//	STOP_EID_FORM(ApplicationType.STOP_EID, "stop_eid_form"),
//	ISSUE_EID_FORM(ApplicationType.ISSUE_EID, "issue_eid_form"),
//	RESUME_EID_FORM(ApplicationType.RESUME_EID, "issue_eid_form");
//	//ISSUE_EID_FORM(ApplicationType.ISSUE_EID, "stop_eid_form");
//	
//    private static final Map<ApplicationType, String> LOOKUP_MAP;
//	
//    static {
//    	Map<ApplicationType, String> tempMap = new EnumMap<>(ApplicationType.class);
//        for (ApplicationTypeToFormMapper mapper : ApplicationTypeToFormMapper.values()) {
//        	tempMap.put(mapper.getApplicationType(), mapper.formName);
//        }
//        
//        LOOKUP_MAP = Collections.unmodifiableMap(tempMap);
//    }
//	
//	private final ApplicationType applicationType;
//	private final String formName;
//	
//	private ApplicationTypeToFormMapper(ApplicationType applicationType, String formName) {
//		this.applicationType = applicationType;
//		this.formName = formName;
//	}
//
//	public static String getFormNameByApplicationType(ApplicationType searchedApplicationType) {
//		return LOOKUP_MAP.get(searchedApplicationType);
//	}
//	
//	public ApplicationType getApplicationType() {
//		return applicationType;
//	}
//
//	public String getFormName() {
//		return formName;
//	}
//}
