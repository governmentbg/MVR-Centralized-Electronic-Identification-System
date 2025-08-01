package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.apigateway.BaseControllerTest;
import bg.bulsi.mvr.apigateway.Factory;
import bg.bulsi.mvr.common.config.PageableConfig;
import bg.bulsi.mvr.mpozei.contract.dto.CertificateRequest;
import bg.bulsi.mvr.mpozei.contract.dto.CertificateResponse;
import bg.bulsi.mvr.mpozei.contract.dto.CitizenCertificateSummaryResponse;
import bg.bulsi.mvr.mpozei.contract.dto.FindCertificateResponse;
import org.junit.jupiter.api.Test;
import org.springframework.context.annotation.Import;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageImpl;
import reactor.core.publisher.Mono;

import java.util.List;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

@Import({PageableConfig.class, CertificateApiDelegateService.class})
public class CertificateDelegateTest extends BaseControllerTest {
    @Test
    void testEnrollPkcs10CertificateShouldReturnOk() {
        CertificateRequest request = Factory.createCertificateRequest();
        CertificateResponse response = Factory.createCertificateResponse();

        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(response));

        webClient
                .post()
                .uri(BASE_URL + "/mpozei/api/v1/certificates/enroll")
                .body(Mono.just(request), request.getClass())
                .header("user-context")
                .exchange()
                .expectStatus()
                .is2xxSuccessful()
                .expectBody(CertificateResponse.class)
                .consumeWith(result -> {
                    assertEquals(response, result.getResponseBody());
                });
    }

    @Test
    void getCitizenCertificateById_Should_Return_Ok() {
        CitizenCertificateSummaryResponse response = new CitizenCertificateSummaryResponse();
        UUID id = UUID.randomUUID();
        response.setId(id);
        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(response));

        webClient
                .get()
                .uri(BASE_URL + "/mpozei/api/v1/certificates/" + id)
                .header("user-context")
                .exchange()
                .expectStatus()
                .is2xxSuccessful()
                .expectBody(CitizenCertificateSummaryResponse.class)
                .consumeWith(result -> {
                    assertEquals(response, result.getResponseBody());
                });
    }

    @Test
    void findCitizenCertificates_Should_Return_Ok() {
        Page<FindCertificateResponse> response = new PageImpl<>(List.of());

        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(response));

        webClient
                .get()
                .uri(BASE_URL + "/mpozei/api/v1/certificates/find?eidentityId=" + UUID.randomUUID())
                .header("user-context")
                .exchange()
                .expectStatus()
                .is2xxSuccessful()
                .expectBody(Page.class)
                .consumeWith(result -> {
                    assertEquals(result.getResponseBody(), response);
                });
    }
}
