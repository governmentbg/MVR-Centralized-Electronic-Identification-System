package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenProfileDTO;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.util.Objects;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertEquals;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertTrue;

@Slf4j
@Component
@RequiredArgsConstructor
public class RueiBaseProfileVerificationStep extends Step<AbstractApplication> {
	private final RaeiceiService raeiceiService;
    private final RueiClient rueiClient;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered RueiBaseProfileVerificationStep", application.getId());

        CitizenProfileDTO dto = getCitizenProfile(application);
        if (Objects.isNull(dto)) {
            return application;
        }

        if (Objects.nonNull(application.getParams().getPhoneNumber())) {
            assertEquals(application.getParams().getPhoneNumber(), dto.getPhoneNumber(),
                    PHONE_NUMBER_IS_DIFFERENT_FROM_EXISTING_ONE);
        } else {
            application.getParams().setPhoneNumber(dto.getPhoneNumber());
        }
        if (Objects.nonNull(application.getParams().getEmail())) {
            assertEquals(application.getParams().getEmail(), dto.getEmail(),
                    EMAIL_IS_DIFFERENT_FROM_EXISTING_ONE);
        } else {
            application.getParams().setEmail(dto.getEmail());
        }

        if (Objects.isNull(UserContextHolder.getFromServletContext().getEidAdministratorId())) {
            assertEquals(dto.getId().toString(), UserContextHolder.getFromServletContext().getCitizenProfileId(), REQUESTER_IS_NOT_OWNER);
        }
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        //EID-7975
        if (Objects.equals(device.getType(), DeviceType.MOBILE)) {
            // TODO: 5/16/2024 check if anything else is needed
            assertTrue(Objects.nonNull(dto.getMobileApplicationInstanceId()), PROFILE_MUST_BE_VERIFIED);
            application.getParams().setMobileApplicationInstanceId(dto.getMobileApplicationInstanceId());
        }
        application.setCitizenProfileId(dto.getId());
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.RUEI_BASE_PROFILE_VERIFICATION;
    }

    private CitizenProfileDTO getCitizenProfile(AbstractApplication application) {
        log.info("Sending request to Get CitizenProfile with eidentityId: {}", application.getEidentityId());
        CitizenProfileDTO profile = null;
        try {
            profile = rueiClient.getCitizenProfileByEidentityId(application.getEidentityId());
        } catch (Exception ignored) {
            log.info("Citizen Profile not found by eidentityId");
        }
        if (Objects.nonNull(profile)) {
            log.info("Received success response for Get CitizenProfile for eidentityId: {}", profile.getEidentityId());
        }
        return profile;
    }
}
