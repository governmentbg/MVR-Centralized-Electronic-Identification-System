//package bg.bulsi.mvr.apigateway.service;
//
//import bg.bulsi.mvr.common.rabbitmq.producer.RabbitControllerAdvice;
//import bg.bulsi.mvr.racei.contract.dto.CitizenDataDTO;
//import bg.bulsi.mvr.racei.contract.dto.EidentityDTO;
//import bg.bulsi.mvr.racei.contract.dto.IdentifierTypeDTO;
//import org.junit.jupiter.api.BeforeEach;
//import org.junit.jupiter.api.Test;
//import org.mockito.Mock;
//import org.mockito.invocation.InvocationOnMock;
//import org.mockito.stubbing.Answer;
//import org.springframework.amqp.core.DirectExchange;
//import org.springframework.amqp.rabbit.AsyncRabbitTemplate;
//import org.springframework.amqp.rabbit.RabbitConverterFuture;
//import org.springframework.amqp.support.converter.RemoteInvocationResult;
//import org.springframework.beans.factory.annotation.Autowired;
//import org.springframework.boot.test.context.SpringBootTest;
//import org.springframework.boot.test.mock.mockito.MockBean;
//import org.springframework.http.server.reactive.ServerHttpRequest;
//import org.springframework.test.context.ActiveProfiles;
//import org.springframework.web.server.ServerWebExchange;
//import reactor.core.publisher.Mono;
//import reactor.test.StepVerifier;
//
//import java.util.Map;
//import java.util.UUID;
//import java.util.concurrent.CompletableFuture;
//import java.util.concurrent.ExecutionException;
//import java.util.function.BiFunction;
//
//import static org.mockito.ArgumentMatchers.any;
//import static org.mockito.ArgumentMatchers.anyString;
//import static org.mockito.Mockito.mock;
//import static org.mockito.Mockito.when;
//
//@ActiveProfiles("logging-dev")
//@SpringBootTest(classes = TestServiceConfig.class)
//class EIdentityApiDelegateServiceTest {
//
//	@MockBean
//	private AsyncRabbitTemplate asyncRabbitTemplate;
//
//	@MockBean(name = "rpcExchange")
//	private DirectExchange rpcExchange;
//
//	@Mock
//	private ServerWebExchange webExchange;
//
//	@Mock
//	private ServerHttpRequest httpRequest;
//
//	private RabbitConverterFuture<Object> rabbitFuture;
//
//	@Autowired
//	private EIdentityApiDelegateService eIdentityApiDelegateService;
//
//	@BeforeEach
//	void setUp() throws Exception {
//		this.rabbitFuture = mock(RabbitConverterFuture.class);
//
//		when(webExchange.getRequest()).thenReturn(httpRequest);
//		when(webExchange.getAttribute(RabbitControllerAdvice.ROUTING_KEY_NAME)).thenReturn("Random_routing_key");
//		when(rpcExchange.getName()).thenReturn("Random_direct_exchange");
//	}
//
//	@Test
//	void testGetEidentityByIdShouldReturn() throws InterruptedException, ExecutionException {
//		EidentityDTO eidentityDTO = new EidentityDTO(UUID.randomUUID(),true, "TestFirstName", "TestSecondName",
//				"TestLastName", "9555555555", IdentifierTypeDTO.EGN);
//
//		when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(UUID.class))).thenReturn(rabbitFuture);
//
//		Answer<CompletableFuture<RemoteInvocationResult>> answer = createCompletableFutureAnswer(new RemoteInvocationResult(eidentityDTO), null);
//
//		when(rabbitFuture.handle(any())).thenAnswer(answer);
//
//		Mono<EidentityDTO> eidentityDTOMono = eIdentityApiDelegateService
//				.getEidentityById(UUID.randomUUID(), webExchange);
//
//		StepVerifier.create(eidentityDTOMono).expectNext(eidentityDTO).expectComplete().verify();
//	}
//
//	@Test
//	void testGetEidentityByIdShouldThrow() throws InterruptedException, ExecutionException {
//		when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(UUID.class))).thenReturn(rabbitFuture);
//
//		Answer<CompletableFuture<RemoteInvocationResult>> answer = createCompletableFutureAnswer(null, new IllegalArgumentException());
//
//		when(rabbitFuture.handle(any())).thenAnswer(answer);
//
//		Mono<EidentityDTO> eidentityDTOMono = eIdentityApiDelegateService
//				.getEidentityById(UUID.randomUUID(), webExchange);
//
//		StepVerifier.create(eidentityDTOMono).expectError(IllegalArgumentException.class).verify();
//	}
//
//	@Test
//	void testUpdateEidentityShouldReturn() throws InterruptedException, ExecutionException {
//		EidentityDTO eidentityDTO = new EidentityDTO(UUID.randomUUID(),true, "TestFirstName", "TestSecondName",
//				"TestLastName", "9555555555", IdentifierTypeDTO.EGN);
//
//		CitizenDataDTO citizenDataDTO = new CitizenDataDTO(null, null, null, null, null);
//
//		when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(Map.class))).thenReturn(rabbitFuture);
//
//		Answer<CompletableFuture<RemoteInvocationResult>> answer = createCompletableFutureAnswer(new RemoteInvocationResult(eidentityDTO), null);
//
//		when(rabbitFuture.handle(any())).thenAnswer(answer);
//
//		Mono<EidentityDTO> eidentityDTOMono = eIdentityApiDelegateService
//				.updateEidentity(UUID.randomUUID(), citizenDataDTO, webExchange);
//
//		StepVerifier.create(eidentityDTOMono).expectNext(eidentityDTO).expectComplete().verify();
//	}
//
//	@Test
//	void testUpdateEidentityShouldThrow() throws InterruptedException, ExecutionException {
//		CitizenDataDTO citizenDataDTO = new CitizenDataDTO(null, null, null, null, null);
//
//		when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(Map.class))).thenReturn(rabbitFuture);
//
//		Answer<CompletableFuture<RemoteInvocationResult>> answer = createCompletableFutureAnswer(null, new IllegalArgumentException());
//
//		when(rabbitFuture.handle(any())).thenAnswer(answer);
//
//		Mono<EidentityDTO> eidentityDTOMono = eIdentityApiDelegateService
//				.updateEidentity(UUID.randomUUID(), citizenDataDTO, webExchange);
//
//		StepVerifier.create(eidentityDTOMono).expectError(IllegalArgumentException.class).verify();
//	}
//
//	@Test
//	void testCreateEidentityShouldReturn() throws InterruptedException, ExecutionException {
//		CitizenDataDTO citizenDataDTO = new CitizenDataDTO(null, null, null, null, null);
//
//		UUID randomUuid = UUID.randomUUID();
//
//		when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(CitizenDataDTO.class))).thenReturn(rabbitFuture);
//
//		Answer<CompletableFuture<RemoteInvocationResult>> answer = createCompletableFutureAnswer(new RemoteInvocationResult(randomUuid), null);
//
//		when(rabbitFuture.handle(any())).thenAnswer(answer);
//
//		Mono<UUID> uuidMono = eIdentityApiDelegateService
//				.createEidentity(citizenDataDTO, webExchange);
//
//		StepVerifier.create(uuidMono).expectNext(randomUuid).expectComplete().verify();
//	}
//
//	@Test
//	void testCreateEidentityShouldThrow() throws InterruptedException, ExecutionException {
//		CitizenDataDTO citizenDataDTO = new CitizenDataDTO(null, null, null, null, null);
//
//		when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(CitizenDataDTO.class))).thenReturn(rabbitFuture);
//
//		Answer<CompletableFuture<RemoteInvocationResult>> answer = createCompletableFutureAnswer(null, new IllegalArgumentException());
//
//		when(rabbitFuture.handle(any())).thenAnswer(answer);
//
//		Mono<UUID> uuidMono = eIdentityApiDelegateService
//				.createEidentity(citizenDataDTO, webExchange);
//
//		StepVerifier.create(uuidMono).expectError(IllegalArgumentException.class).verify();
//	}
//
//	private Answer<CompletableFuture<RemoteInvocationResult>> createCompletableFutureAnswer(RemoteInvocationResult result, Throwable throwable) {
//		return new Answer<CompletableFuture<RemoteInvocationResult>>() {
//
//			public CompletableFuture<RemoteInvocationResult> answer(InvocationOnMock invocation) throws Throwable {
//				Object stage = invocation.getArgument(0);
//
//				BiFunction<RemoteInvocationResult, Throwable, Void> biFunction = (BiFunction<RemoteInvocationResult, Throwable, Void>) stage;
//				biFunction.apply(result, throwable);
//
//				return new CompletableFuture<RemoteInvocationResult>();
//			}
//		};
//	}
//}
