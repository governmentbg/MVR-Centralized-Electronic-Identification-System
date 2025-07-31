package bg.bulsi.mvr.extgateway.service;

import org.mapstruct.Mapper;
import org.mapstruct.NullValueCheckStrategy;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.mpozei.contract.dto.OnlineApplicationRequest;
import bg.bulsi.mvr.mpozei.contract.dto.OnlineApplicationResponse;
import bg.bulsi.mvr.mpozei.contract.dto.OnlineCertStatusApplicationRequest;
import lombok.extern.slf4j.Slf4j;

@Component
@Mapper(componentModel = "spring",
        nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public class  ApplicationAuditMapper {

//	public  EventPayload mapToEventPayload (OnlineApplicationResponse response);
//	
//	public  EventPayload mapToEventPayload (OnlineCertStatusApplicationRequest request);
		 
}
