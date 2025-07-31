package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.common.config.PageableConfig;
import bg.bulsi.mvr.extgateway.BaseControllerTest;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationDetailsResponse;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Import;
import reactor.core.publisher.Mono;

import java.util.UUID;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

@Import({PageableConfig.class, ApplicationApiDelegateService.class})
public class ApplicationDelegateTest extends BaseControllerTest {
    @Autowired
    private ApplicationApiDelegateService applicationApiDelegate;

//    @Test
//    void createApplication_Should_Return_Ok() {
//        ApplicationRequest request = Factory.createApplicationRequest();
//        ApplicationResponse response = Factory.createApplicationResponse();
//
//        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(response));
//
//        webClient
//                .post()
//                .uri(BASE_URL + "/mpozei/external/api/v1/applications")
//                .body(Mono.just(request), request.getClass())
//                .header("user-context")
//                .exchange()
//                .expectStatus()
//                .isOk()
//                .expectBody(ApplicationResponse.class)
//                .consumeWith(result -> {
//                    assertEquals(response, result.getResponseBody());
//                });
//
//    }

//    @Test
//    void findApplicationsByFilter_Should_Return_Ok() {
//        Page<ApplicationDTO> applications = new PageImpl<>(Collections.emptyList());
//        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(applications));
//
//        webClient
//                .get()
//                .uri(BASE_URL + "/mpozei/external/api/v1/applications/find?eidentityId=32310025-4cc2-46a4-b03a-a869543da4d2&page=0&size=10")
//                .header("user-context")
//                .exchange()
//                .expectStatus()
//                .isOk()
//                .expectBody(Page.class)
//                .consumeWith(result -> {
//                    assertEquals(result.getResponseBody(), applications);
//                });
//    }

    @Test
    void getApplicationById_Should_Return_Ok() {
        ApplicationDetailsResponse response = new ApplicationDetailsResponse();
        UUID id = UUID.randomUUID();
        response.setId(id);
        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(response));

        webClient
                .get()
                .uri(BASE_URL + "/mpozei/external/api/v1/applications/" + id)
                .header("user-context")
                .exchange()
                .expectStatus()
                .isOk()
                .expectBody(ApplicationDetailsResponse.class)
                .consumeWith(result -> {
                    assertEquals(result.getResponseBody(), response);
                });
    }
}
