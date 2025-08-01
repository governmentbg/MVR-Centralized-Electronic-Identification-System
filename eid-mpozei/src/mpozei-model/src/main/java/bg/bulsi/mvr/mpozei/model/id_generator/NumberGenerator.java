package bg.bulsi.mvr.mpozei.model.id_generator;

import bg.bulsi.mvr.mpozei.model.id_generator.id.ApplicationNumber;
import lombok.extern.slf4j.Slf4j;
import org.hibernate.HibernateException;
import org.hibernate.LockMode;
import org.hibernate.MappingException;
import org.hibernate.engine.spi.SharedSessionContractImplementor;
import org.hibernate.id.Configurable;
import org.hibernate.id.IdentifierGenerator;
import org.hibernate.internal.SessionImpl;
import org.hibernate.service.ServiceRegistry;
import org.hibernate.type.Type;

import java.io.Serializable;
import java.time.Clock;
import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.util.Properties;
import java.util.UUID;

@Slf4j
public class NumberGenerator implements IdentifierGenerator, Configurable {
    private static final DateTimeFormatter DATE_FORMAT = DateTimeFormatter.ofPattern("dd.MM.yyyy");
//    private UUID numberCounterId;

    @Override
    public void configure(Type type, Properties params, ServiceRegistry serviceRegistry) throws MappingException {
//        numberCounterId = UUID.fromString(params.getProperty(NumberCounter.NUMBER_COUNTER_ID));
//        log.debug("Hibernate properties access test: {}", numberCounterId);
    }

    @Override
    public Serializable generate(SharedSessionContractImplementor session, Object object) throws HibernateException {
        try {
            ApplicationNumber application = (ApplicationNumber) object;
            if ((application.getId() != null) && !application.getId().trim().isEmpty()) {
                return application.getId();
            }
            
            NumberCounterId cId = new NumberCounterId(application.getAdministratorCode(), application.getOfficeCode());
//            NumberCounter sCounter = new NumberCounter();
//            sCounter.setNumberCounterId(cId);
            SessionImpl sessionImpl = (SessionImpl) session;
//            boolean hasCounter = sessionImpl.contains(sCounter);
            NumberCounter counter = sessionImpl.get(NumberCounter.class, cId, LockMode.PESSIMISTIC_WRITE);

            int sequence;
            try {
                log.debug("Current transaction: Active-{}; Joined-{}", sessionImpl.getTransactionCoordinator().isActive(),
                        sessionImpl.getTransactionCoordinator().isJoined());
                log.debug("Transaction auto join: {}", sessionImpl.shouldAutoJoinTransaction());
//                log.info("Session contains [{}] - {}",cId, counter);
               if (counter==null) {
                   //sessionImpl.persist(sCounter);
                   counter = new NumberCounter();
                   counter.setNumberCounterId(cId);
                   log.info("Create New NumberCounter record: {}",counter);
                   
                } 
//                counter = sessionImpl.get(NumberCounter.class, cId , LockMode.PESSIMISTIC_WRITE);
                sequence = counter.getNextCount();
                counter = sessionImpl.merge(counter);

//                log.debug("Counter LockMode = {}", sessionImpl.getCurrentLockMode(counter));
            } catch (Exception e) {
                throw new HibernateException("CANNOT INITIALIZE SEQUENCE NUMBER", e); 
            }

            // build string builder with application prefix
            StringBuilder sb = new StringBuilder(counter.getAdministratorCode());

            sb.append("-").append(counter.getOfficeCode()).append("-");
            // make the index to 9 digits with leading 0s
            sb.append(String.format("%09d", sequence));

            // append delimiter
            sb.append("/");

            /*
             add date
             */
            LocalDate currentDate = LocalDate.now(Clock.systemUTC());
            sb.append(currentDate.format(DATE_FORMAT));

//            transaction.commit();
            return sb.toString();

        } catch (Exception e) {
            throw new HibernateException("SEQUENCE GENERATOR EXCEPTION", e);
        }
    }

 
}