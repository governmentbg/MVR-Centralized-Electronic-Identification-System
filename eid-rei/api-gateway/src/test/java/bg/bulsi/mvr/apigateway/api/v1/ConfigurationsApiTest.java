/**
 * 
 */
package bg.bulsi.mvr.apigateway.api.v1;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

import java.time.OffsetDateTime;

import org.junit.jupiter.api.Test;
import org.openapitools.jackson.nullable.JsonNullable;
import org.openapitools.jackson.nullable.JsonNullableModule;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.TestConfiguration;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Primary;
import org.springframework.http.converter.json.Jackson2ObjectMapperBuilder;
import org.springframework.test.web.reactive.server.WebTestClient;

import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;

import bg.bulsi.mvr.reicontract.dto.ConfigurationResultDTO;
import bg.bulsi.mvr.reicontract.dto.UpdateConfigurationRequestDTO;
import reactor.core.publisher.Mono;

/**
 * 
 */
class ConfigurationsApiTest extends BaseControllerTest {

    @Autowired
    private WebTestClient webClient;
	
    @Autowired
    private ConfigurationsApi configurationsApi;
    
    @MockBean
    private ConfigurationsApiDelegate configurationsApiDelegate;
    
	/**
	 * Test method for {@link bg.bulsi.mvr.apigateway.api.v1.ConfigurationsApi#getDelegate()}.
	 */
	@Test
	final void testGetDelegateShouldReturnNotNull() {
		assertNotNull(configurationsApi.getDelegate());
	}

	/**
	 * Test method for {@link bg.bulsi.mvr.apigateway.api.v1.ConfigurationsApi#getConfigByKey(java.lang.String, org.springframework.web.server.ServerWebExchange)}.
	 */
	@Test
	final void testGetConfigByKeyShouldReturnOk() {
		ConfigurationResultDTO configurationResultDTOExpected = 
				ConfigurationResultDTO.builder()
				.key(JsonNullable.of("TestKey"))
				.data(JsonNullable.of("TestData"))
				.modifiedOn(JsonNullable.of(OffsetDateTime.now()))
				.modifiedBy(JsonNullable.of("TestModifedBy"))
				.build();
		
		when(configurationsApiDelegate.getConfigByKey(any(), any())).thenReturn(Mono.just(configurationResultDTOExpected));
		
	    webClient
        .get()
        .uri("http://localhost:8090/api/v1/configuration/TestKey")
        .exchange()
        .expectStatus()
        .isOk()
        .expectBody(ConfigurationResultDTO.class)
        .consumeWith(result -> {
        	ConfigurationResultDTO configurationResultDTOActual = result.getResponseBody();
        	
        	assertEquals(configurationResultDTOExpected.getKey(), configurationResultDTOActual.getKey());
        	assertEquals(configurationResultDTOExpected.getData(), configurationResultDTOActual.getData());
        	assertEquals(configurationResultDTOExpected.getModifiedOn().get().toInstant(), configurationResultDTOActual.getModifiedOn().get().toInstant());
        	assertEquals(configurationResultDTOExpected.getModifiedBy(), configurationResultDTOActual.getModifiedBy());
        });
	}

	/**
	 * Test method for {@link bg.bulsi.mvr.apigateway.api.v1.ConfigurationsApi#updateConfig(java.lang.String, bg.bulsi.mvr.reicontract.dto.UpdateConfigurationRequestDTO, org.springframework.web.server.ServerWebExchange)}.
	 */
	@Test
	final void testUpdateConfigShouldReturnOk() {
		when(configurationsApiDelegate.updateConfig(any(), any(), any())).thenReturn(Mono.empty());
		
	    webClient
        .put()
        .uri("http://localhost:8090/api/v1/configuration/TestKey")
        .body(Mono.just(new UpdateConfigurationRequestDTO()), UpdateConfigurationRequestDTO.class)
        .exchange()
        .expectStatus()
        .isNoContent()
        .expectBody(Void.class);
	}

	@TestConfiguration
	public static class InnerTestConfig {
		@Bean
		@Primary
		public Jackson2ObjectMapperBuilder customObjectMapper() {
			return new Jackson2ObjectMapperBuilder().modules(
					new JsonNullableModule(), 
					new JavaTimeModule());
		}
	}
}
