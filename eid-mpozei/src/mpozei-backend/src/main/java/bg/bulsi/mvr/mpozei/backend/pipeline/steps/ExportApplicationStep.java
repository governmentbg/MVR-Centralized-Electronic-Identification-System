package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.backend.service.QrCodeService;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.pdf_generator.PdfGenerator;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorDTO;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.util.Map;

@Slf4j
@Component
@RequiredArgsConstructor
public class ExportApplicationStep extends Step<AbstractApplication> {
	
	public static final String BULGARIA_COUNTRY_NAME = "България";
	
	private final ApplicationMapper applicationMapper;
    private final PdfGenerator pdfGenerator;
    private final QrCodeService qrCodeService;
    private final RaeiceiService raeiceiService;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered ExportApplicationStep", application.getId());
 
        Map<String, Object> formParams = applicationMapper.mapToKeyValueMap(application);
        formParams.putAll(applicationMapper.mapToKeyValueMapFromFE());
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        if (device.getType().equals(DeviceType.MOBILE) && !application.getParams().getRequireGuardians()) {
            EidAdministratorDTO eidAdministrator = raeiceiService.getEidAdministratorById(application.getEidAdministratorId());
            formParams.put("qrCode", qrCodeService.generateQrCodeImage(application.getId(), eidAdministrator.getNameLatin()));
        }
        String templateName = application.getApplicationType().name();
//        if (!BULGARIA_COUNTRY_NAME.equalsIgnoreCase(application.getCitizenship().trim())) {
//            templateName = templateName + "_FOREIGNER";
//        }
        templateName = templateName + ".html";

        byte[] pdfFile = pdfGenerator.generatePdf(templateName, formParams);
        
        application.getTemporaryData().setApplicationExportResponse(pdfFile);
        
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.EXPORT_APPLICATION;
    }
}
