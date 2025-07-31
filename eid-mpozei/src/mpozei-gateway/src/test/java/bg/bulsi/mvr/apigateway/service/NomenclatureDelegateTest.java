package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.apigateway.BaseControllerTest;
import bg.bulsi.mvr.common.config.PageableConfig;
import bg.bulsi.mvr.mpozei.contract.dto.NomenclatureDTO;
import org.junit.jupiter.api.Test;
import org.springframework.context.annotation.Import;
import reactor.core.publisher.Mono;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

@Import({PageableConfig.class, NomenclatureApiDelegateService.class})
public class NomenclatureDelegateTest extends BaseControllerTest {

    @Test
    void getAllReasons_Should_Return_Ok() {
        List<NomenclatureDTO> response = new ArrayList<>();
        NomenclatureDTO dto = new NomenclatureDTO();
        UUID id = UUID.randomUUID();
        dto.setId(id);
        response.add(dto);

        when(eventSender.send(any(), any(), any(), any())).thenReturn(Mono.just(response));

        webClient
                .get()
                .uri(BASE_URL + "/mpozei/api/v1/nomenclatures/reasons")
                .header("user-context")
                .exchange()
                .expectStatus()
                .isOk()
                .expectBody(List.class)
                .consumeWith(result -> {
                    assertEquals(response.get(0).getId().toString(), ((LinkedHashMap) result.getResponseBody().get(0)).get("id"));
                });
    }
}
