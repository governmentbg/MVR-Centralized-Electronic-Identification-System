package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.EncryptionHelper;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.common.rabbitmq.producer.ExtendedRemoteInvocationResult;
import bg.bulsi.mvr.common.rabbitmq.producer.RabbitControllerAdvice;
import bg.bulsi.mvr.reicontract.dto.CitizenDataDTO;
import bg.bulsi.mvr.reicontract.dto.EidentityDTO;
import bg.bulsi.mvr.reicontract.dto.IdentifierTypeDTO;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.MockedStatic;
import org.mockito.Mockito;
import org.mockito.invocation.InvocationOnMock;
import org.mockito.stubbing.Answer;
import org.springframework.amqp.core.DirectExchange;
import org.springframework.amqp.core.MessageProperties;
import org.springframework.amqp.rabbit.AsyncRabbitTemplate;
import org.springframework.amqp.rabbit.RabbitConverterFuture;
import org.springframework.amqp.support.converter.RemoteInvocationResult;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.http.HttpHeaders;
import org.springframework.http.server.reactive.ServerHttpRequest;
import org.springframework.http.server.reactive.ServerHttpResponse;
import org.springframework.test.context.ActiveProfiles;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;
import reactor.test.StepVerifier;

import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
import java.util.function.BiFunction;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

@ActiveProfiles("logging-dev")
@SpringBootTest(classes = TestServiceConfig.class)
class EIdentityApiDelegateServiceTest {
	
	@MockBean
	private AsyncRabbitTemplate asyncRabbitTemplate;

	@MockBean(name = "rpcExchange")
	private DirectExchange rpcExchange;

	@Mock
	private ServerWebExchange webExchange;

	@Mock
	private ServerHttpRequest httpRequest;
	
	@Mock
	private ServerHttpResponse httpResponse;

	@Mock
	private MessageProperties messageProperties;
	
	private RabbitConverterFuture<Object> rabbitFuture;
	
	@Autowired
	private EIdentityApiDelegateService eIdentityApiDelegateService;

	@BeforeEach
	void setUp() throws Exception {
		this.rabbitFuture = mock(RabbitConverterFuture.class);

		when(messageProperties.getHeader(any())).thenReturn("{}");
		when(webExchange.getRequest()).thenReturn(httpRequest);
		when(webExchange.getResponse()).thenReturn(httpResponse);
		when(httpResponse.getHeaders()).thenReturn(new HttpHeaders());
		when(webExchange.getAttribute(RabbitControllerAdvice.ROUTING_KEY_NAME)).thenReturn("Random_routing_key");
		when(rpcExchange.getName()).thenReturn("Random_direct_exchange");
	}

	@Test
	void testGetEidentityByIdShouldReturn() throws InterruptedException, ExecutionException {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromReactiveContext)
            .thenReturn(Mono.just(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(), "af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc",true, "")));
			EidentityDTO eidentityDTO = new EidentityDTO(UUID.randomUUID(), true, "TestFirstName", "TestSecondName",
					"TestLastName", "9555555555", IdentifierTypeDTO.EGN);

			when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(UUID.class),any())).thenReturn(rabbitFuture);

			RemoteInvocationResult remoteInvocationResult = new RemoteInvocationResult(eidentityDTO);
			Answer<CompletableFuture<ExtendedRemoteInvocationResult>> answer = 
					createCompletableFutureAnswer(new ExtendedRemoteInvocationResult(remoteInvocationResult, messageProperties));

			when(rabbitFuture.handle(any())).thenAnswer(answer);

			Mono<EidentityDTO> eidentityDTOMono = eIdentityApiDelegateService
					.getEidentityById(UUID.randomUUID(), webExchange);

			StepVerifier.create(eidentityDTOMono).expectNext(eidentityDTO).expectComplete().verify();
		}
	}
	
	@Test
	void testGetEidentityByIdShouldThrow() throws InterruptedException, ExecutionException {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromReactiveContext)
            .thenReturn(Mono.just(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(), "af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc",true, "")));
			when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(UUID.class), any())).thenReturn(rabbitFuture);

			RemoteInvocationResult remoteInvocationResult = new RemoteInvocationResult(new IllegalArgumentException());
			Answer<CompletableFuture<ExtendedRemoteInvocationResult>> answer = createCompletableFutureAnswer(new ExtendedRemoteInvocationResult(remoteInvocationResult, messageProperties));

			when(rabbitFuture.handle(any())).thenAnswer(answer);

			Mono<EidentityDTO> eidentityDTOMono = eIdentityApiDelegateService
					.getEidentityById(UUID.randomUUID(), webExchange);

			StepVerifier.create(eidentityDTOMono).expectError(IllegalArgumentException.class).verify();
		}
	}
	
	@Test
	void testUpdateEidentityShouldReturn() throws InterruptedException, ExecutionException {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromReactiveContext)
            .thenReturn(Mono.just(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(), "af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc",true, "")));
			EidentityDTO eidentityDTO = new EidentityDTO(UUID.randomUUID(), true, "TestFirstName", "TestSecondName",
					"TestLastName", "9555555555", IdentifierTypeDTO.EGN);

			CitizenDataDTO citizenDataDTO = new CitizenDataDTO(null, null, null, null, null);

			when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(Map.class), any())).thenReturn(rabbitFuture);

			RemoteInvocationResult remoteInvocationResult = new RemoteInvocationResult(eidentityDTO);
			Answer<CompletableFuture<ExtendedRemoteInvocationResult>> answer = createCompletableFutureAnswer(new ExtendedRemoteInvocationResult(remoteInvocationResult, messageProperties));

			when(rabbitFuture.handle(any())).thenAnswer(answer);

			Mono<EidentityDTO> eidentityDTOMono = eIdentityApiDelegateService
					.updateEidentity(UUID.randomUUID(), citizenDataDTO, webExchange);

			StepVerifier.create(eidentityDTOMono).expectNext(eidentityDTO).expectComplete().verify();
		}
	}
	
	@Test
	void testUpdateEidentityShouldThrow() throws InterruptedException, ExecutionException {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromReactiveContext)
            .thenReturn(Mono.just(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(), "af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc",true, "")));
			CitizenDataDTO citizenDataDTO = new CitizenDataDTO(null, null, null, null, null);

			when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(Map.class), any())).thenReturn(rabbitFuture);

			RemoteInvocationResult remoteInvocationResult = new RemoteInvocationResult(new IllegalArgumentException());
			Answer<CompletableFuture<ExtendedRemoteInvocationResult>> answer = createCompletableFutureAnswer(new ExtendedRemoteInvocationResult(remoteInvocationResult, messageProperties));

			when(rabbitFuture.handle(any())).thenAnswer(answer);

			Mono<EidentityDTO> eidentityDTOMono = eIdentityApiDelegateService
					.updateEidentity(UUID.randomUUID(), citizenDataDTO, webExchange);

			StepVerifier.create(eidentityDTOMono).expectError(IllegalArgumentException.class).verify();
		}
	}
	
	@Test
	void testCreateEidentityShouldReturn() throws InterruptedException, ExecutionException {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromReactiveContext)
            .thenReturn(Mono.just(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(), "af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc",true, "")));
			CitizenDataDTO citizenDataDTO = new CitizenDataDTO(null, null, null, null, null);

			UUID randomUuid = UUID.randomUUID();

			when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(CitizenDataDTO.class), any())).thenReturn(rabbitFuture);

			RemoteInvocationResult remoteInvocationResult = new RemoteInvocationResult(randomUuid);
			Answer<CompletableFuture<ExtendedRemoteInvocationResult>> answer = createCompletableFutureAnswer(new ExtendedRemoteInvocationResult(remoteInvocationResult, messageProperties));

			when(rabbitFuture.handle(any())).thenAnswer(answer);

			Mono<UUID> uuidMono = eIdentityApiDelegateService
					.createEidentity(citizenDataDTO, webExchange);

			StepVerifier.create(uuidMono).expectNext(randomUuid).expectComplete().verify();
		}
	}
	
	@Test
	void testCreateEidentityShouldThrow() throws InterruptedException, ExecutionException {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromReactiveContext)
            .thenReturn(Mono.just(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(), "af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc",true, "")));
			CitizenDataDTO citizenDataDTO = new CitizenDataDTO(null, null, null, null, null);

			when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(CitizenDataDTO.class), any())).thenReturn(rabbitFuture);

			RemoteInvocationResult remoteInvocationResult = new RemoteInvocationResult(new IllegalArgumentException());
			Answer<CompletableFuture<ExtendedRemoteInvocationResult>> answer = createCompletableFutureAnswer(new ExtendedRemoteInvocationResult(remoteInvocationResult, messageProperties));

			when(rabbitFuture.handle(any())).thenAnswer(answer);

			Mono<UUID> uuidMono = eIdentityApiDelegateService
					.createEidentity(citizenDataDTO, webExchange);

			StepVerifier.create(uuidMono).expectError(IllegalArgumentException.class).verify();
		}
	}
	
	@Test
	void testUpdateEidentityActiveShouldReturn() throws InterruptedException, ExecutionException {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromReactiveContext)
            .thenReturn(Mono.just(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(), "af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc",true, "")));
			EidentityDTO eidentityDTO = new EidentityDTO(UUID.randomUUID(), true, "TestFirstName", "TestSecondName",
					"TestLastName", "9555555555", IdentifierTypeDTO.EGN);

			when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(Map.class), any())).thenReturn(rabbitFuture);

			RemoteInvocationResult remoteInvocationResult = new RemoteInvocationResult(eidentityDTO);
			Answer<CompletableFuture<ExtendedRemoteInvocationResult>> answer = createCompletableFutureAnswer(new ExtendedRemoteInvocationResult(remoteInvocationResult, messageProperties));

			when(rabbitFuture.handle(any())).thenAnswer(answer);

			Mono<EidentityDTO> eidentityDTOMono = eIdentityApiDelegateService
					.updateEidentityActive(UUID.randomUUID(), true, webExchange);

			StepVerifier.create(eidentityDTOMono).expectNext(eidentityDTO).expectComplete().verify();
		}
	}
	
	@Test
	void testUpdateEidentityActiveShouldThrow() throws InterruptedException, ExecutionException {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromReactiveContext)
            .thenReturn(Mono.just(new UserContext("", "", "", "", "", "", "", "", List.of(""), UUID.randomUUID(), "af7c1fe6-d669-414e-b066-e9733f0de7a8", "194a90a0-3b9d-47f5-865a-ad8bcf2c3acc",true, "")));
			when(asyncRabbitTemplate.convertSendAndReceive(anyString(), anyString(), any(Map.class), any())).thenReturn(rabbitFuture);

			RemoteInvocationResult remoteInvocationResult = new RemoteInvocationResult(new IllegalArgumentException());
			Answer<CompletableFuture<ExtendedRemoteInvocationResult>> answer = createCompletableFutureAnswer(new ExtendedRemoteInvocationResult(remoteInvocationResult, messageProperties));

			when(rabbitFuture.handle(any())).thenAnswer(answer);

			Mono<EidentityDTO> eidentityDTOMono = eIdentityApiDelegateService
					.updateEidentityActive(UUID.randomUUID(), true, webExchange);

			StepVerifier.create(eidentityDTOMono).expectError(IllegalArgumentException.class).verify();
		}
	}
	
	private Answer<CompletableFuture<ExtendedRemoteInvocationResult>> createCompletableFutureAnswer(ExtendedRemoteInvocationResult result) {
		return new Answer<CompletableFuture<ExtendedRemoteInvocationResult>>() {

			public CompletableFuture<ExtendedRemoteInvocationResult> answer(InvocationOnMock invocation) throws Throwable {
				Object stage = invocation.getArgument(0);

				BiFunction<ExtendedRemoteInvocationResult, Throwable, Void> biFunction = (BiFunction<ExtendedRemoteInvocationResult, Throwable, Void>) stage;
				biFunction.apply(result, null);
				
				return new CompletableFuture<ExtendedRemoteInvocationResult>();
			}
		};
	}
}
