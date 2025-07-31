package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenProfileAttachDTO;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenProfileDTO;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.backend.service.ApplicationService;
import bg.bulsi.mvr.mpozei.backend.service.impl.ApplicationServiceImpl;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.repository.ApplicationRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.DependsOn;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Component;

import java.util.List;
import java.util.Objects;
import java.util.UUID;

import static bg.bulsi.mvr.common.util.ValidationUtil.assertNotNull;

@Slf4j
@Component
@RequiredArgsConstructor
public class RueiBaseProfileAttachmentStep extends Step<AbstractApplication> {
    private final RueiClient rueiClient;
    private final ApplicationMapper applicationMapper;
    private final ApplicationRepository<AbstractApplication> applicationRepository;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered RueiBaseProfileAttachmentStep", application.getId());
        
        CitizenProfileAttachDTO attachDto = applicationMapper.mapToCitizenProfileAttachDTO(application);
        
        try {
        	CitizenProfileDTO profileDto = attachCitizenProfile(attachDto);
        	
        	List<AbstractApplication> applications = applicationRepository.findAllByEidentityId(profileDto.getEidentityId(), Pageable.unpaged())
                    .stream()
                    .filter(e -> Objects.isNull(e.getCitizenProfileId())).toList();
            applications.forEach(e -> e.setCitizenProfileId(profileDto.getId()));
            applicationRepository.saveAll(applications);
        } catch (Exception exception) {
			log.error(".process() ", "Could not attachCitizenProfile for Application with [id={}] [citizenProfileId={}] [eid={}] ", 
					application.getId(), 
					application.getCitizenProfileId(),
					application.getEidentityId());
			
			return application;
        }
        
      //  assertNotNull(profileDto, ErrorCode.CITIZEN_PROFILE_CANNOT_BE_ATTACHED);

        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.RUEI_BASE_PROFILE_ATTACHMENT;
    }

    private CitizenProfileDTO attachCitizenProfile(CitizenProfileAttachDTO attachDto) {
        String citizenProfileId = UserContextHolder.getFromServletContext().getCitizenProfileId();
        attachDto.setCitizenProfileId(UUID.fromString(citizenProfileId));

        log.info("Sending request to attach CitizenProfile to EIdentity with eidentityId: {}", attachDto.getEidentityId());
        CitizenProfileDTO profile = rueiClient.attachCitizenProfile(attachDto);
        log.info("Received success response for CitizenProfile attachment for eidentityId: {}", profile.getEidentityId());
        return profile;
    }
}
