package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.apigateway.BaseControllerTest;
import bg.bulsi.mvr.common.config.PageableConfig;
import bg.bulsi.mvr.mpozei.contract.dto.EidentityResponse;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Import;
import reactor.core.publisher.Mono;

import java.util.UUID;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

@Import({PageableConfig.class, EidentityApiDelegateService.class})
public class EidentityDelegateTest extends BaseControllerTest {

    @Autowired
    private EidentityApiDelegateService eidentityApiDelegateService;

    @Test
    void getEidentityByIdentifierNumberAndIdentifierType_Should_Return_Ok() {
        EidentityResponse response = new EidentityResponse();
        response.setEidentityId(UUID.randomUUID());

        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(response));

        webClient
                .get()
                .uri(BASE_URL + "/mpozei/api/v1/eidentities/find?number=1234&type=EGN")
                .header("user-context")
                .exchange()
                .expectStatus()
                .isOk()
                .expectBody(EidentityResponse.class)
                .consumeWith(result -> {
                    assertEquals(response, result.getResponseBody());
                });
    }
}
