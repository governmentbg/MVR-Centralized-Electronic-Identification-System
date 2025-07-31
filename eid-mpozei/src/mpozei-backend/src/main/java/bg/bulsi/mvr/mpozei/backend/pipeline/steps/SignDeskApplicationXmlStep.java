package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.common.service.FileFormatService;
import bg.bulsi.mvr.mpozei.backend.mapper.XmlMapper;
import bg.bulsi.mvr.mpozei.contract.dto.xml.EidApplicationXml;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

@Slf4j
@Component
@RequiredArgsConstructor
public class SignDeskApplicationXmlStep extends Step<AbstractApplication> {
    private final FileFormatService fileFormatService;
    private final XmlMapper xmlMapper;
    
    @Override
    @Transactional
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered XmlSignatureCreationStep", application.getId());

        EidApplicationXml eidApplicationXml = this.fileFormatService.createObjectFromXmlString(application.getApplicationXml(), EidApplicationXml.class);
        eidApplicationXml = this.xmlMapper.map(eidApplicationXml, application.getTemporaryData());
        eidApplicationXml.setEmail(application.getParams().getEmail());
        eidApplicationXml.setPhoneNumber(application.getParams().getPhoneNumber());

        application.setApplicationXml(this.fileFormatService.createXmlStringFromObject(eidApplicationXml));

        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.SIGNATURE_CREATION;
    }
}
