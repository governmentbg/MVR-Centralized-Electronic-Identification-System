package bg.bulsi.mvr.mpozei.backend.model;

import bg.bulsi.mvr.mpozei.backend.BaseTest;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.application.IssueEidApplication;
import bg.bulsi.mvr.mpozei.model.repository.ApplicationRepository;
import lombok.extern.slf4j.Slf4j;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.TestInstance;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.stream.IntStream;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

@Slf4j
@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class ApplicationNumberTests extends BaseTest {
    @Autowired
    private ApplicationRepository<AbstractApplication> abstractApplicationRepository;

    @Test
    void testConcurrentSave() throws InterruptedException {
        int threadCount = 100;
        ExecutorService executorService = Executors.newFixedThreadPool(threadCount);
        CountDownLatch latch = new CountDownLatch(threadCount);

        IntStream.range(0, threadCount).forEach(i -> {
            executorService.submit(() -> {
                createApplication();
                latch.countDown();
            });
        });

        latch.await();
        executorService.shutdown();

        List<AbstractApplication> applications = abstractApplicationRepository.findAll();
        boolean haveDifferentAppNumbers = applications.stream().map(AbstractApplication::getApplicationNumber).distinct().toList().size() == applications.size();
        assertTrue(haveDifferentAppNumbers);
    }

    @Transactional
    public void createApplication() {
        IssueEidApplication application = new IssueEidApplication();
        application.setCitizenIdentifierNumber("9012035678");
        abstractApplicationRepository.save(application);
        log.info("created new application");
    }
}
