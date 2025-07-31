package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.common.service.FileFormatService;
import bg.bulsi.mvr.mpozei.backend.mapper.XmlMapper;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.xml.EidApplicationXml;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.application.TemporaryData;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.PERSO_CENTRE;

@Slf4j
@Component
@RequiredArgsConstructor
public class CreateDeskApplicationXmlStep extends Step<AbstractApplication> {
    private final FileFormatService fileFormatService;
    private final XmlMapper xmlMapper;
    
    @Override
    @Transactional
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered XmlSignatureCreationStep", application.getId());
        
        TemporaryData temporaryData = application.getTemporaryData();
        EidApplicationXml eidApplicationXml = this.xmlMapper.map(application, temporaryData.getPersonalIdentityDocument());

        ApplicationSubmissionType submissionType = application.getSubmissionType();
        if (submissionType == PERSO_CENTRE) {
            eidApplicationXml = this.xmlMapper.map(eidApplicationXml, temporaryData);
            eidApplicationXml.setEmail(application.getParams().getEmail());
            eidApplicationXml.setPhoneNumber(application.getParams().getPhoneNumber());
        }

    	//application.setSignedDetails(eidSignatureXml.toString());
        application.setApplicationXml(this.fileFormatService.createXmlStringFromObject(eidApplicationXml));

        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.SIGNATURE_CREATION;
    }
}
