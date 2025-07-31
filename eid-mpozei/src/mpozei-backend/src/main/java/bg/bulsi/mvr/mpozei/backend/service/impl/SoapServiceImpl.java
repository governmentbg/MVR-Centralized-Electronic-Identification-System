package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.mpozei.backend.config.soap_handlers.SoapLoggingHandler;
import bg.bulsi.mvr.mpozei.backend.service.SoapService;
import bg.bulsi.mvr.mpozei.backend.soap_impl.DataPreparationIfc;
import bg.bulsi.mvr.mpozei.backend.soap_impl.DataPreparationResult;
import bg.bulsi.mvr.mpozei.backend.soap_impl.DataPreparationService;
import jakarta.annotation.PostConstruct;
import jakarta.xml.ws.BindingProvider;
import jakarta.xml.ws.handler.Handler;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.PUK_CANNOT_BE_OBTAINED;

@Service
@Slf4j
public class SoapServiceImpl implements SoapService {
    @Value("${wsdl-url}")
    private String wsdlUrl;

    private final static String PUK_XML_REQUEST =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            "<Dp3RequestPUK DatasetId=\"%s\">" +
                "<ChipSerialNumber>%s</ChipSerialNumber>" +
                "<HolderId>%s</HolderId>" +
            "</Dp3RequestPUK>";
    private final static String PUK_TYPE = "DP3_PUK";
    private DataPreparationService dataPreparationService;

    @Override
    public byte[] prepareDataAsByteArray(String chipSerialNumber, String holderId) {
        String xml = String.format(PUK_XML_REQUEST, UUID.randomUUID(), chipSerialNumber, holderId);
        log.info("WSDL url:{}", wsdlUrl);
        DataPreparationIfc port = dataPreparationService.getDataPreparationWebserviceSoap11Port();
        BindingProvider bindingProvider = (BindingProvider) port;
        List<Handler> handlerChain = new ArrayList<>();
        handlerChain.add(new SoapLoggingHandler());
        bindingProvider.getBinding().setHandlerChain(handlerChain);

//        ((BindingProvider) port).getRequestContext().put(
//                BindingProvider.ENDPOINT_ADDRESS_PROPERTY,
//                wsdlUrl
//        );
        DataPreparationResult result = port.prepareDataAsByteArray(xml.getBytes(), PUK_TYPE);
        log.info("result byte data:{}", result.getByteData());
        log.info("result isSuccess:{}", result.isSuccess());

        if (!result.isSuccess()) {
            log.error("Error code: {}, Message: {}, Location: {}", result.getErrorCode(), result.getErrorMessage(), result.getErrorLocation());
            throw new FaultMVRException(result.getErrorMessage(), PUK_CANNOT_BE_OBTAINED);
        }
        return result.getByteData();
    }

    @PostConstruct
    private void initDataPreparationService() {
        this.dataPreparationService = createDataPreparationService();
    }

    private DataPreparationService createDataPreparationService() {
        try {
            return new DataPreparationService(new URL(wsdlUrl));
        } catch (MalformedURLException e) {
            throw new FaultMVRException(e);
        }
    }
}
