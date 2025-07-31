package bg.bulsi.mvr.mpozei.backend.config.soap_handlers;

import jakarta.xml.soap.SOAPException;
import jakarta.xml.soap.SOAPMessage;
import jakarta.xml.ws.handler.MessageContext;
import jakarta.xml.ws.handler.soap.SOAPHandler;
import jakarta.xml.ws.handler.soap.SOAPMessageContext;
import lombok.extern.slf4j.Slf4j;

import javax.xml.namespace.QName;
import java.io.IOException;
import java.util.Set;

@Slf4j
public class SoapLoggingHandler implements SOAPHandler<SOAPMessageContext> {

    @Override
    public boolean handleMessage(SOAPMessageContext context) {
        Boolean isOutbound = (Boolean) context.get(MessageContext.MESSAGE_OUTBOUND_PROPERTY);
        SOAPMessage soapMessage = context.getMessage();
        try {
            if (isOutbound) {
                System.out.println("Outbound SOAP Message:");
            } else {
                System.out.println("Inbound SOAP Message:");
            }
            soapMessage.writeTo(System.out);
            System.out.println(); // Newline for better formatting
        } catch (Exception e) {
            log.error(e.getMessage(), e);
        }
        return true; // Continue processing
    }

    @Override
    public boolean handleFault(SOAPMessageContext context) {
        try {
            context.getMessage().writeTo(System.out);
        } catch (SOAPException | IOException e) {
            throw new RuntimeException(e);
        }
        return true;
    }

    @Override
    public void close(MessageContext context) {
        // Cleanup resources if necessary
    }

    @Override
    public Set<QName> getHeaders() {
        // Return the set of QNames for headers processed by this handler
        return null;
    }
}

