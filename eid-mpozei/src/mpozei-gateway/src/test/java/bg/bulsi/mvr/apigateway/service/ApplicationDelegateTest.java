package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.apigateway.BaseControllerTest;
import bg.bulsi.mvr.apigateway.Factory;
import bg.bulsi.mvr.common.config.PageableConfig;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationDTO;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationDetailsResponse;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSignRequest;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Import;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageImpl;
import reactor.core.publisher.Mono;

import java.util.Collections;
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
//    	try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
//			userContextHolder.when(UserContextHolder::getFromReactiveContext)
//					.thenReturn(Mono.just(new UserContext("", "", "", "", "", "", "", List.of(""), UUID.randomUUID(), true)));
//			
//        ApplicationRequest request = Factory.createApplicationRequest();
//        ApplicationResponse response = Factory.createApplicationResponse();
//        
//        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(response));
//
//        webClient
//                .post()
//                .uri(BASE_URL + "/mpozei/api/v1/applications")
//                .body(Mono.just(request), request.getClass())
//                .header("user-context")
//                .exchange()
//                .expectStatus()
//                .isOk()
//                .expectBody(ApplicationResponse.class)
//                .consumeWith(result -> {
//                    assertEquals(response, result.getResponseBody());
//                });
//		}
//    }

    @Test
    void exportApplication_Should_Return_Ok() {
        byte[] response = Factory.createByteArray();
        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(response));

        webClient
                .get()
                .uri(BASE_URL + "/mpozei/api/v1/applications/{applicationId}/export", UUID.randomUUID())
                .header("user-context")
                .exchange()
                .expectStatus()
                .isOk()
                .expectBody(byte[].class)
                .consumeWith(result -> {
                    assertEquals(new String(response), new String(result.getResponseBody()));
                });
    }

    @Test
    void signApplication_Should_Return_Ok() {
        ApplicationStatus status = ApplicationStatus.SIGNED;
        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(status));

        webClient
                .post()
                .uri(BASE_URL + "/mpozei/api/v1/applications/c3f15ecf-5f59-422a-ae0a-7f5dc72b9ebb/sign")
                .header("user-context")
                .bodyValue(new ApplicationSignRequest())
                .exchange()
                .expectStatus()
                .isOk()
                .expectBody(ApplicationStatus.class)
                .consumeWith(result -> {
                    assertEquals(result.getResponseBody(), status);
                });
    }

    @Test
    void updateApplicationStatus_Should_Return_Ok() {
        ApplicationStatus status = ApplicationStatus.COMPLETED;
        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(status));

        webClient
                .post()
                .uri(BASE_URL + "/mpozei/api/v1/applications/{applicationId}/status?status=" + status.name(), UUID.randomUUID())
                .header("user-context")
                .exchange()
                .expectStatus()
                .isOk()
                .expectBody(ApplicationStatus.class)
                .consumeWith(result -> {
                    assertEquals(result.getResponseBody(), status);
                });
    }

    @Test
    void findApplicationsByFilter_Should_Return_Ok() {
        Page<ApplicationDTO> applications = new PageImpl<>(Collections.emptyList());
        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(applications));

        webClient
                .get()
                .uri(BASE_URL + "/mpozei/api/v1/applications/find?eidentityId=32310025-4cc2-46a4-b03a-a869543da4d2&page=0&size=10")
                .header("user-context")
                .exchange()
                .expectStatus()
                .isOk()
                .expectBody(Page.class)
                .consumeWith(result -> {
                    assertEquals(result.getResponseBody(), applications);
                });
    }

    @Test
    void getApplicationById_Should_Return_Ok() {
        ApplicationDetailsResponse response = new ApplicationDetailsResponse();
        UUID id = UUID.randomUUID();
        response.setId(id);
        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(response));

        webClient
                .get()
                .uri(BASE_URL + "/mpozei/api/v1/applications/" + id)
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
