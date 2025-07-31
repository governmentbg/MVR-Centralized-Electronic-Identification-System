package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.raeicei.backend.service.CheckCodeService;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerFrontOfficeRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

@Slf4j
@Service
@RequiredArgsConstructor
public class CheckCodeServiceImpl implements CheckCodeService {

    private final EidManagerRepository eidManagerRepository;
    private final EidManagerFrontOfficeRepository eidManagerFrontOfficeRepository;

    @Override
    public void checkCode(String code, Boolean isOffice) {

        if (!isOffice && eidManagerRepository.existsByCode(code)) {
            throw new ValidationMVRException(ErrorCode.EID_MANAGER_WITH_THIS_CODE_ALREADY_EXISTS);
        }

        if (isOffice && eidManagerFrontOfficeRepository.existsByCode(code)) {
            throw new ValidationMVRException(ErrorCode.EID_MANAGER_FRONT_OFFICE_WITH_THIS_CODE_ALREADY_EXISTS);
        }
    }
}
