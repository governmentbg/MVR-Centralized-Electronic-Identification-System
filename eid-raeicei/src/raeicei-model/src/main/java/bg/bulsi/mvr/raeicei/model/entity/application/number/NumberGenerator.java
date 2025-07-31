package bg.bulsi.mvr.raeicei.model.entity.application.number;

import bg.bulsi.mvr.raeicei.model.entity.application.ApplicationNumber;
import bg.bulsi.mvr.raeicei.model.enums.ApplicationType;
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
	
    private static final long serialVersionUID = 3959539764448032711L;
	private static final DateTimeFormatter DATE_FORMAT = DateTimeFormatter.ofPattern("dd.MM.yyyy");
    private UUID numberCounterId;

    @Override
    public void configure(Type type, Properties params, ServiceRegistry serviceRegistry) throws MappingException {
        log.debug("Hibernate properties access test: {}", numberCounterId);
    }

    @Override
    public Serializable generate(SharedSessionContractImplementor session, Object object) throws HibernateException {
        try {
            ApplicationNumber applicationNumber = (ApplicationNumber) object;
            if (applicationNumber.getId() != null) {
                return applicationNumber;
            }

            SessionImpl sessionImpl = (SessionImpl) session;
            NumberCounter counter = sessionImpl.get(NumberCounter.class, applicationNumber.getApplicationType(), LockMode.PESSIMISTIC_WRITE);

            int sequence;
            try {
                log.debug("Current transaction: Active-{}; Joined-{}", sessionImpl.getTransactionCoordinator().isActive(),
                        sessionImpl.getTransactionCoordinator().isJoined());
                log.debug("Transaction auto join: {}", sessionImpl.shouldAutoJoinTransaction());
                if (counter == null) {
                    sequence = createCounter(sessionImpl,applicationNumber.getApplicationType());
                } else {
                    sequence = counter.getNextCount();
                    sessionImpl.merge(counter);
                }
//                log.debug("Counter LockMode = {}", sessionImpl.getCurrentLockMode(counter));
            } catch (Exception e) {
                throw new HibernateException("CANNOT INITIALIZE SEQUENCE NUMBER", e);
            }

            // build string builder with application prefix
            StringBuilder sb = new StringBuilder();
            
            sb.append(applicationNumber.getApplicationType().getPrefix());
            sb.append("-");
            // make the index to 9 digits with leading 0s
            sb.append(String.format("%05d", sequence));
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

    private synchronized int createCounter(SessionImpl session, ApplicationType appType) {
        NumberCounter counter = new NumberCounter(appType);
        int sequence = counter.getNextCount();
        session.persist(counter);
        return sequence;
    }
}