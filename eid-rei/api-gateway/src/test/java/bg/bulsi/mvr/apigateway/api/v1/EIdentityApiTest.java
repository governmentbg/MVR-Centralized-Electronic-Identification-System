/**
 *
 */
package bg.bulsi.mvr.apigateway.api.v1;

import bg.bulsi.mvr.reicontract.dto.CitizenDataDTO;
import bg.bulsi.mvr.reicontract.dto.EidentityDTO;
import bg.bulsi.mvr.reicontract.dto.IdentifierTypeDTO;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ProblemDetail;
import org.springframework.test.web.reactive.server.WebTestClient;
import reactor.core.publisher.Mono;

import java.util.UUID;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

class EIdentityApiTest extends BaseControllerTest {

    @Autowired
    private WebTestClient webClient;

    @MockBean
    private EIdentityApiDelegate eIdentityApiDelegate;

    private static CitizenDataDTO citizenDataDTO;

    private static EidentityDTO eidentityDTOExcepted;

	/**
	 * @throws java.lang.Exception
	 */
	@BeforeAll
	static void setUpBeforeClass() throws Exception {
		citizenDataDTO = new CitizenDataDTO("TestFirstName", "TestSecondName",
				"TestLastName", "9555555555", IdentifierTypeDTO.EGN);

		 eidentityDTOExcepted = new EidentityDTO(UUID.randomUUID(),true, "TestFirstName", "TestSecondName",
				"TestLastName", "9555555555", IdentifierTypeDTO.EGN);
	}

	/**
	 * Test method for {@link bg.bulsi.mvr.apigateway.api.v1.EIdentityApi#createEidentity(bg.bulsi.mvr.reicontract.dto.CitizenDataDTO, org.springframework.web.server.ServerWebExchange)}.
	 */
	@Test
//	@WithMockUser
	void testCreateEidentityShouldReturnOk() {
//		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
//			userContextHolder.when(UserContextHolder::getFromReactiveContext)
//					.thenReturn(Mono.just(new UserContext("", "", List.of(""), UUID.randomUUID())));
		// TODO: 1/2/2024 fix test
//			UUID randomUuid = UUID.randomUUID();
//
//			when(eIdentityApiDelegate.createEidentity(any(), any())).thenReturn(Mono.just(randomUuid));
//
//			webClient
//					.post()
//					.uri("http://localhost:8090/api/v1/eidentity")
//					.body(Mono.just(citizenDataDTO), citizenDataDTO.getClass())
//					.header("user-context")
//					.exchange()
//					.expectStatus()
//					.isOk()
//					.expectBody(UUID.class)
//					.consumeWith(result -> {
//						assertEquals(randomUuid, result.getResponseBody());
//					});
//		}
	}

	/**
	 * Test method for {@link bg.bulsi.mvr.apigateway.api.v1.EIdentityApi#createEidentity(bg.bulsi.mvr.reicontract.dto.CitizenDataDTO, org.springframework.web.server.ServerWebExchange)}.
	 */
	@Test
	void testCreateEidentityShouldThrowException() {
		// TODO: 1/2/2024 fix test
//		when(eIdentityApiDelegate.createEidentity(any(), any())).thenReturn(Mono.error(new NullPointerException()));
//
//	    webClient
//        .post()
//        .uri("http://localhost:8090/api/v1/eidentity")
//        .body(Mono.just(citizenDataDTO), citizenDataDTO.getClass())
//        .exchange()
//        .expectStatus()
//        .is5xxServerError()
//        .expectBody(ProblemDetail.class)
//        .consumeWith(result -> {
//        	ProblemDetail problemDetailActual = result.getResponseBody();
//
//        	assertEquals("/api/v1/eidentity", problemDetailActual.getInstance().toString());
//        	assertEquals(HttpStatus.INTERNAL_SERVER_ERROR.value(), problemDetailActual.getStatus());
//        	assertEquals("about:blank", problemDetailActual.getType().toString());
//        	assertEquals("Internal Server Error", problemDetailActual.getTitle());
//        	assertEquals("An unexpected error occurred.", problemDetailActual.getDetail());
//        	assertEquals(null, problemDetailActual.getProperties());
//        });
	}

	/**
	 * Test method for {@link bg.bulsi.mvr.apigateway.api.v1.EIdentityApi#getEidentityById(java.util.UUID, org.springframework.web.server.ServerWebExchange)}.
	 */
	@Test
	void testGetEidentityByIdShouldReturnOk() {
		when(eIdentityApiDelegate.getEidentityById(any(), any())).thenReturn(Mono.just(eidentityDTOExcepted));

	    webClient
        .get()
        .uri("http://localhost:8090/api/v1/eidentity/964ca2f4-c516-455d-84eb-3667a15b4200")
        .exchange()
        .expectStatus()
        .isOk()
        .expectBody(EidentityDTO.class)
        .consumeWith(result -> {
    		EidentityDTO eidentityDTOActual = result.getResponseBody();

        	assertEquals(eidentityDTOExcepted.getFirstName(), eidentityDTOActual.getFirstName());
        	assertEquals(eidentityDTOExcepted.getSecondName(), eidentityDTOActual.getSecondName());
        	assertEquals(eidentityDTOExcepted.getLastName(), eidentityDTOActual.getLastName());
        	assertEquals(eidentityDTOExcepted.getCitizenIdentifierNumber(), eidentityDTOActual.getCitizenIdentifierNumber());
        	assertEquals(eidentityDTOExcepted.getCitizenIdentifierType(), eidentityDTOActual.getCitizenIdentifierType());
        	assertEquals(eidentityDTOExcepted.getActive(), eidentityDTOActual.getActive());
        });
	}

	/**
	 * Test method for {@link bg.bulsi.mvr.apigateway.api.v1.EIdentityApi#getEidentityById(java.util.UUID, org.springframework.web.server.ServerWebExchange)}.
	 */
	@Test
	void testGetEidentityByIdShouldThrowException() {
		when(eIdentityApiDelegate.getEidentityById(any(), any())).thenReturn(Mono.error(new NullPointerException()));

	    webClient
        .get()
        .uri("http://localhost:8090/api/v1/eidentity/964ca2f4-c516-455d-84eb-3667a15b4200")
        .exchange()
        .expectStatus()
        .is5xxServerError()
        .expectBody(ProblemDetail.class)
        .consumeWith(result -> {
        	ProblemDetail problemDetailActual = result.getResponseBody();

        	assertEquals("/api/v1/eidentity/964ca2f4-c516-455d-84eb-3667a15b4200", problemDetailActual.getInstance().toString());
        	assertEquals(HttpStatus.INTERNAL_SERVER_ERROR.value(), problemDetailActual.getStatus());
        	assertEquals("about:blank", problemDetailActual.getType().toString());
        	assertEquals("Internal Server Error", problemDetailActual.getTitle());
        	assertEquals("An unexpected error occurred.", problemDetailActual.getDetail());
        	assertEquals(null, problemDetailActual.getProperties());
        });
	}

	/**
	 * Test method for {@link bg.bulsi.mvr.apigateway.api.v1.EIdentityApi#updateEidentity(java.util.UUID, bg.bulsi.mvr.reicontract.dto.CitizenDataDTO, org.springframework.web.server.ServerWebExchange)}.
	 */
	@Test
	void testUpdateEidentityShouldReturnOk() {
		// TODO: 1/2/2024 fix test
//		when(eIdentityApiDelegate.updateEidentity(any(), any(), any())).thenReturn(Mono.just(eidentityDTOExcepted));
//
//	    webClient
//        .put()
//        .uri("http://localhost.mvr.bg:8090/api/v1/eidentity/964ca2f4-c516-455d-84eb-3667a15b4200")
//        .body(Mono.just(citizenDataDTO), citizenDataDTO.getClass())
//        .accept(MediaType.APPLICATION_JSON)
//        .exchange()
//        .expectStatus()
//        .isOk()
//        .expectBody(EidentityDTO.class)
//        .consumeWith(result -> {
//    		EidentityDTO eidentityDTOActual = result.getResponseBody();
//
//        	assertEquals(eidentityDTOExcepted.getFirstName(), eidentityDTOActual.getFirstName());
//        	assertEquals(eidentityDTOExcepted.getSecondName(), eidentityDTOActual.getSecondName());
//        	assertEquals(eidentityDTOExcepted.getLastName(), eidentityDTOActual.getLastName());
//        	assertEquals(eidentityDTOExcepted.getCitizenIdentifierNumber(), eidentityDTOActual.getCitizenIdentifierNumber());
//        	assertEquals(eidentityDTOExcepted.getCitizenIdentifierType(), eidentityDTOActual.getCitizenIdentifierType());
//        	assertEquals(eidentityDTOExcepted.getActive(), eidentityDTOActual.getActive());
//        });
	}

	/**
	 * Test method for {@link bg.bulsi.mvr.apigateway.api.v1.EIdentityApi#updateEidentity(java.util.UUID, bg.bulsi.mvr.reicontract.dto.CitizenDataDTO, org.springframework.web.server.ServerWebExchange)}.
	 */
	@Test
	void testUpdateEidentityShouldThrowException() {
		// TODO: 1/2/2024 fix test
//		when(eIdentityApiDelegate.updateEidentity(any(), any(), any())).thenReturn(Mono.error(new NullPointerException()));
//
//	    webClient
//        .put()
//        .uri("http://localhost.mvr.bg:8090/api/v1/eidentity/964ca2f4-c516-455d-84eb-3667a15b4200")
//        .body(Mono.just(citizenDataDTO), citizenDataDTO.getClass())
//        .accept(MediaType.APPLICATION_JSON)
//        .exchange()
//        .expectStatus()
//        .is5xxServerError()
//        .expectBody(ProblemDetail.class)
//        .consumeWith(result -> {
//        	ProblemDetail problemDetailActual = result.getResponseBody();
//
//        	assertEquals("/api/v1/eidentity/964ca2f4-c516-455d-84eb-3667a15b4200", problemDetailActual.getInstance().toString());
//        	assertEquals(HttpStatus.INTERNAL_SERVER_ERROR.value(), problemDetailActual.getStatus());
//        	assertEquals("about:blank", problemDetailActual.getType().toString());
//        	assertEquals("Internal Server Error", problemDetailActual.getTitle());
//        	assertEquals("An unexpected error occurred.", problemDetailActual.getDetail());
//        	assertEquals(null, problemDetailActual.getProperties());
//        });
	}

	/**
	 * Test method for {@link bg.bulsi.mvr.apigateway.api.v1.EIdentityApi#updateEidentityActive()}.
	 */
	@Test
	void testUpdateEidentityActiveShouldReturnOk() {
		when(eIdentityApiDelegate.updateEidentityActive(any(), any(), any())).thenReturn(Mono.just(eidentityDTOExcepted));

	    webClient
        .put()
        .uri("http://localhost.mvr.bg:8090/api/v1/eidentity/964ca2f4-c516-455d-84eb-3667a15b4200/status?isActive=true")
        .accept(MediaType.APPLICATION_JSON)
        .exchange()
        .expectStatus()
        .isOk()
        .expectBody(EidentityDTO.class)
        .consumeWith(result -> {
    		EidentityDTO eidentityDTOActual = result.getResponseBody();

        	assertEquals(eidentityDTOExcepted.getFirstName(), eidentityDTOActual.getFirstName());
        	assertEquals(eidentityDTOExcepted.getSecondName(), eidentityDTOActual.getSecondName());
        	assertEquals(eidentityDTOExcepted.getLastName(), eidentityDTOActual.getLastName());
        	assertEquals(eidentityDTOExcepted.getCitizenIdentifierNumber(), eidentityDTOActual.getCitizenIdentifierNumber());
        	assertEquals(eidentityDTOExcepted.getCitizenIdentifierType(), eidentityDTOActual.getCitizenIdentifierType());
        	assertEquals(eidentityDTOExcepted.getActive(), eidentityDTOActual.getActive());
        });
	}

	/**
	 * Test method for {@link bg.bulsi.mvr.apigateway.api.v1.EIdentityApi#updateEidentityActive()}.
	 */
	@Test
	void testUpdateEidentityActiveShouldThrowException() {
		when(eIdentityApiDelegate.updateEidentityActive(any(), any(), any())).thenReturn(Mono.error(new NullPointerException()));

	    webClient
        .put()
        .uri("http://localhost.mvr.bg:8090/api/v1/eidentity/964ca2f4-c516-455d-84eb-3667a15b4200/status?isActive=true")
        .accept(MediaType.APPLICATION_JSON)
        .exchange()
        .expectStatus()
        .is5xxServerError()
        .expectBody(ProblemDetail.class)
        .consumeWith(result -> {
        	ProblemDetail problemDetailActual = result.getResponseBody();

        	assertEquals("/api/v1/eidentity/964ca2f4-c516-455d-84eb-3667a15b4200/status", problemDetailActual.getInstance().toString());
        	assertEquals(HttpStatus.INTERNAL_SERVER_ERROR.value(), problemDetailActual.getStatus());
        	assertEquals("about:blank", problemDetailActual.getType().toString());
        	assertEquals("Internal Server Error", problemDetailActual.getTitle());
        	assertEquals("An unexpected error occurred.", problemDetailActual.getDetail());
        	assertEquals(null, problemDetailActual.getProperties());
        });
	}
}
