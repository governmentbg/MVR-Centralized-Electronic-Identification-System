package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.raeicei.backend.BaseTest;
import bg.bulsi.mvr.raeicei.backend.mapper.DeviceMapper;
import bg.bulsi.mvr.raeicei.backend.mapper.EidManagerFrontOfficeMapper;
import bg.bulsi.mvr.raeicei.backend.mapper.EidManagerMapper;
import bg.bulsi.mvr.raeicei.backend.mapper.TariffMapper;
import bg.bulsi.mvr.raeicei.backend.rabbitmq.EidManagerListenerDispatcher;
import bg.bulsi.mvr.raeicei.backend.rabbitmq.FrontOfficeListenerDispatcher;
import bg.bulsi.mvr.raeicei.backend.rabbitmq.ListenerDispatcher;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.EidManagerFrontOffice;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import bg.bulsi.mvr.raeicei.model.entity.Discount;
import bg.bulsi.mvr.raeicei.model.entity.EidAdministrator;
import bg.bulsi.mvr.raeicei.model.entity.tariif.*;
import bg.bulsi.mvr.raeicei.model.entity.tariif.Tariff;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.springframework.amqp.support.converter.RemoteInvocationResult;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.mock.mockito.MockBean;

import java.time.LocalDate;
import java.util.List;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

public class ListenerDispatcherTests extends BaseTest {

    @Autowired
    private ListenerDispatcher listenerDispatcher;

    @Autowired
    private EidManagerListenerDispatcher eidManagerListenerDispatcher;
    
    @Autowired
    private FrontOfficeListenerDispatcher frontOfficeListenerDispatcher;
    
    @MockBean
    private EidAdministratorService eidAdministratorService;

    @MockBean
    private EidManagerMapper eidManagerMapper;

    @MockBean
    private TariffService tariffService;

//    @MockBean
//    private TariffService deviceTariffService;

    @MockBean
    private DiscountService discountService;

    @MockBean
    private EidManagerFrontOfficeService frontOfficeService;

    @MockBean
    private DeviceService deviceService;

    @MockBean
    private DeviceMapper deviceMapper;

    @MockBean
    private TariffMapper tariffMapper;

    @MockBean
    private EidManagerFrontOfficeMapper administratorFrontOfficeMapper;
    private EidAdministratorDTO inputDto;
    private EidAdministrator testEntity;
    private EidAdministratorDTO expectedDto;
    private EidManagerFrontOffice testEntityAdmin;
    private EidManagerFrontOfficeDTO expectedDtoAdmin;
    private Device testEntityDevice;
    private DeviceDTO expectedDtoDevice;
    private Tariff testEntityTariff;
    private TariffDTO expectedDtoTariff;
    private DeviceTariff testEntityDeviceTariff;
    private DeviceTariffDTO expectedDtoDeviceTariff;
    private Discount testEntityDiscount;
    private DiscountDTO expectedDtoDiscount;
    private DiscountDateDTO expectedDtoDiscountDate;

    @BeforeEach
    public void setUp() {
        inputDto = new EidAdministratorDTO();
        testEntity = new EidAdministrator();
        expectedDto = new EidAdministratorDTO();
        testEntityAdmin = new EidManagerFrontOffice();
        expectedDtoAdmin = new EidManagerFrontOfficeDTO();
        testEntityDevice = new Device();
        expectedDtoDevice = new DeviceDTO();
        testEntityTariff = new DeviceTariff();
        expectedDtoTariff = new TariffDTO();
        testEntityDeviceTariff = new DeviceTariff();
        expectedDtoDeviceTariff = new DeviceTariffDTO();
        expectedDtoDiscountDate = new DiscountDateDTO();
        testEntityDiscount = new Discount();
    }

    @Test
    public void testGetEidAdministratorById() {
        UUID testId = UUID.randomUUID();

        when(eidAdministratorService.getEidAdministratorById(testId)).thenReturn(testEntity);

        RemoteInvocationResult result = eidManagerListenerDispatcher.getEidAdministratorById(testId);

        assertNotNull(result);
    }

    @Test
    public void testGetEidAdministratorById_EntityNotFound() {
        UUID testId = UUID.randomUUID();
        String entityName = "EidAdministrator";

        when(eidAdministratorService.getEidAdministratorById(testId))
                .thenThrow(new EntityNotFoundException(ErrorCode.EID_ADMINISTRATOR_NOT_FOUND, entityName, testId));

        EntityNotFoundException exception = assertThrows(EntityNotFoundException.class, () -> {
            eidAdministratorService.getEidAdministratorById(testId);
        });

        String expectedMessage = String.format(ErrorCode.EID_ADMINISTRATOR_NOT_FOUND.getDetail(), entityName, testId);

        String actualMessage = exception.getMessage();
        assertTrue(actualMessage.contains(expectedMessage), "Actual message does not match the expected error message");

        verify(eidAdministratorService).getEidAdministratorById(testId);
        verifyNoMoreInteractions(eidAdministratorService, eidManagerMapper);
    }

    @Test
    public void testCreateEidAdministrator() {
        when(eidAdministratorService.createEidAdministrator(inputDto)).thenReturn(testEntity);
        when(eidManagerMapper.mapToAdministratorDto(testEntity)).thenReturn(expectedDto);

        RemoteInvocationResult result = eidManagerListenerDispatcher.createEidAdministrator(inputDto);

        assertNotNull(result);
        Assertions.assertEquals(expectedDto, result.getValue());
    }

    @Test
    public void testUpdateEidAdministrator() {
        when(eidAdministratorService.updateEidAdministrator(inputDto)).thenReturn(testEntity);
        when(eidManagerMapper.mapToAdministratorDto(testEntity)).thenReturn(expectedDto);

        RemoteInvocationResult result = eidManagerListenerDispatcher.updateEidAdministrator(inputDto);

        assertNotNull(result);
        Assertions.assertEquals(expectedDto, result.getValue());
    }

    @Test
    public void testGetAllEidAdministrators() {
        List<EidAdministratorDTO> expectedDtos = List.of(new EidAdministratorDTO());

        when(eidAdministratorService.getAllEidAdministrators()).thenReturn(expectedDtos);

        RemoteInvocationResult result = eidManagerListenerDispatcher.getAllEidAdministrators();

        assertNotNull(result);
        assertEquals(expectedDtos, result.getValue());
    }

    @Test
    public void testGetAdministratorFrontOfficeById() {
        UUID testId = UUID.randomUUID();

        when(frontOfficeService.getEidManagerFrontOfficeById(testId)).thenReturn(testEntityAdmin);
        when(administratorFrontOfficeMapper.mapToDto(testEntityAdmin)).thenReturn(expectedDtoAdmin);

        RemoteInvocationResult result = frontOfficeListenerDispatcher.getEidManagerFrontOfficeById(testId);

        assertNotNull(result);
        Assertions.assertEquals(expectedDtoAdmin, result.getValue());
        Assertions.assertEquals(expectedDtoAdmin.getId(), ((EidManagerFrontOfficeDTO) result.getValue()).getId());

        verify(frontOfficeService).getEidManagerFrontOfficeById(testId);
        verify(administratorFrontOfficeMapper).mapToDto(testEntityAdmin);
        verifyNoMoreInteractions(frontOfficeService, administratorFrontOfficeMapper);
    }

    @Test
    public void testAdministratorFrontOfficeGetById_EntityNotFound() {
        UUID testId = UUID.randomUUID();
        String entityName = "AdministratorFrontOffice";

        when(frontOfficeService.getEidManagerFrontOfficeById(testId))
                .thenThrow(new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, entityName, testId.toString()));

        EntityNotFoundException exception = assertThrows(EntityNotFoundException.class, () -> {
        	frontOfficeListenerDispatcher.getEidManagerFrontOfficeById(testId);
        });

        String expectedMessage = String.format(ErrorCode.ENTITY_NOT_FOUND.getDetail(), entityName, testId.toString());

        String actualMessage = exception.getMessage();
        assertTrue(actualMessage.contains(expectedMessage), "Actual message does not match the expected error message");

        verify(frontOfficeService).getEidManagerFrontOfficeById(testId);
        verifyNoMoreInteractions(frontOfficeService, administratorFrontOfficeMapper);
    }

//    @Test
//    public void testGetAllAdministratorFrontOfficesByEidAdministratorId() {
//        UUID testId = UUID.randomUUID();
//
//        when(frontOfficeService.getAllEidManagerFrontOfficesByEidManagerId(testId)).thenReturn(List.of(testEntityAdmin));
//        when(administratorFrontOfficeMapper.mapToDtoList(List.of(testEntityAdmin))).thenReturn(List.of(expectedDtoAdmin));
//
//        RemoteInvocationResult result = frontOfficeListenerDispatcher.getAllEidManagerFrontOfficesByEidManagerId(testId);
//
//        assertNotNull(result);
//        Assertions.assertEquals(List.of(expectedDtoAdmin), result.getValue());
//
//        verify(frontOfficeService).getAllEidManagerFrontOfficesByEidManagerId(testId);
//        verify(administratorFrontOfficeMapper).mapToDtoList(List.of(testEntityAdmin));
//        verifyNoMoreInteractions(frontOfficeService, administratorFrontOfficeMapper);
//    }

//    @Test
//    public void testCreateAdministratorFrontOffice() {
//        when(frontOfficeService.createEidManagerFrontOffice(expectedDtoAdmin)).thenReturn(testEntityAdmin);
//        when(administratorFrontOfficeMapper.mapToDto(testEntityAdmin)).thenReturn(expectedDtoAdmin);
//
//        RemoteInvocationResult result = frontOfficeListenerDispatcher.createEidManagerFrontOffice(expectedDtoAdmin);
//
//        assertNotNull(result);
//        Assertions.assertEquals(expectedDtoAdmin, result.getValue());
//    }

//    @Test
//    public void testUpdateAdministratorFrontOffice() {
//        when(frontOfficeService.updateEidManagerFrontOffice(expectedDtoAdmin)).thenReturn(testEntityAdmin);
//        when(administratorFrontOfficeMapper.mapToDto(testEntityAdmin)).thenReturn(expectedDtoAdmin);
//
//        RemoteInvocationResult result = frontOfficeListenerDispatcher.updateEidManagerFrontOffice(expectedDtoAdmin);
//
//        assertNotNull(result);
//        Assertions.assertEquals(expectedDtoAdmin, result.getValue());
//    }

    @Test
    public void testGetDeviceById() {
        UUID testId = UUID.randomUUID();

        when(deviceService.getDeviceById(testId)).thenReturn(testEntityDevice);
        when(deviceMapper.mapToDto(testEntityDevice)).thenReturn(expectedDtoDevice);

        RemoteInvocationResult result = listenerDispatcher.getDeviceById(testId);

        assertNotNull(result);
        Assertions.assertEquals(expectedDtoDevice, result.getValue());
        Assertions.assertEquals(expectedDtoDevice.getId(), ((DeviceDTO) result.getValue()).getId());

        verify(deviceService).getDeviceById(testId);
        verify(deviceMapper).mapToDto(testEntityDevice);
        verifyNoMoreInteractions(deviceService, deviceMapper);
    }

    @Test
    public void testGetDeviceById_EntityNotFound() {
        UUID testId = UUID.randomUUID();
        String entityName = "Device";

        when(deviceService.getDeviceById(testId))
                .thenThrow(new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, entityName, testId.toString()));

        EntityNotFoundException exception = assertThrows(EntityNotFoundException.class, () -> {
            listenerDispatcher.getDeviceById(testId);
        });

        String expectedMessage = String.format(ErrorCode.ENTITY_NOT_FOUND.getDetail(), entityName, testId.toString());

        String actualMessage = exception.getMessage();
        assertTrue(actualMessage.contains(expectedMessage), "Actual message does not match the expected error message");

        verify(deviceService).getDeviceById(testId);
        verifyNoMoreInteractions(deviceService, deviceMapper);
    }

//    @Test
//    public void testGetAllDevicesByAdministratorFrontOfficeId() {
//        when(deviceService.getAllDevices()).thenReturn(List.of(testEntityDevice));
//        when(deviceMapper.mapToDtoList(List.of(testEntityDevice))).thenReturn(List.of(expectedDtoDevice));
//
//        RemoteInvocationResult result = listenerDispatcher.getAllDevices();
//
//        assertNotNull(result);
//        Assertions.assertEquals(List.of(expectedDtoDevice), result.getValue());
//
//        verify(deviceService).getAllDevices();
//        verify(deviceMapper).mapToDtoList(List.of(testEntityDevice));
//        verifyNoMoreInteractions(deviceService, deviceMapper);
//    }

//    @Test
//    public void testCreateDevice() {
//        when(deviceService.createDevice(expectedDtoDevice)).thenReturn(testEntityDevice);
//        when(deviceMapper.mapToDto(testEntityDevice)).thenReturn(expectedDtoDevice);
//
//        RemoteInvocationResult result = listenerDispatcher.createDevice(expectedDtoDevice);
//
//        assertNotNull(result);
//        Assertions.assertEquals(expectedDtoDevice, result.getValue());
//    }

//    @Test
//    public void testGetTariffByDateAndEidAdministrator() {
//        TariffDateDTO tariffDTO = new TariffDateDTO();
//        tariffDTO.setEidManagerId(UUID.randomUUID());
//        tariffDTO.setDate(LocalDate.now());
//
//        when(tariffService.getTariffByDateAndEidManagerId(tariffDTO)).thenReturn(testEntityTariff);
//        when(tariffMapper.mapToTariffDto(testEntityTariff)).thenReturn(expectedDtoTariff);
//
//        RemoteInvocationResult result = listenerDispatcher.getTariffByDateAndEidManagerId(tariffDTO);
//
//        assertNotNull(result);
//        Assertions.assertEquals(expectedDtoTariff, result.getValue());
//        Assertions.assertEquals(expectedDtoTariff.getId(), ((TariffDTO) result.getValue()).getId());
//        Assertions.assertEquals(expectedDtoTariff.getEidManagerId(), ((TariffDTO) result.getValue()).getEidManagerId());
//        Assertions.assertEquals(expectedDtoTariff.getStartDate(), ((TariffDTO) result.getValue()).getStartDate());
//
//    }

    @Test
    public void testGetTariffByDateAndEidAdministrator_EntityNotFound() {
        TariffDateDTO tariffDTO = new TariffDateDTO();
        tariffDTO.setEidManagerId(UUID.randomUUID());
        tariffDTO.setDate(LocalDate.now());
        String entityName = "Tariff";

        when(tariffService.getTariffByDateAndEidManagerId(tariffDTO))
                .thenThrow(new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, entityName, tariffDTO.getEidManagerId().toString()));

        EntityNotFoundException exception = assertThrows(EntityNotFoundException.class, () -> {
            listenerDispatcher.getTariffByDateAndEidManagerId(tariffDTO);
        });

        String expectedMessage = String.format(ErrorCode.ENTITY_NOT_FOUND.getDetail(), entityName, tariffDTO.getEidManagerId().toString());

        String actualMessage = exception.getMessage();
        assertTrue(actualMessage.contains(expectedMessage), "Actual message does not match the expected error message");

        verify(tariffService).getTariffByDateAndEidManagerId(tariffDTO);
        verifyNoMoreInteractions(tariffService, tariffMapper);
    }

//    @Test
//    public void testCreateTariff() {
//        when(tariffService.createTariff(expectedDtoTariff)).thenReturn(testEntityTariff);
//        when(tariffMapper.mapToTariffDto(testEntityTariff)).thenReturn(expectedDtoTariff);
//
//        RemoteInvocationResult result = listenerDispatcher.createTariff(expectedDtoTariff);
//
//        assertNotNull(result);
//        Assertions.assertEquals(expectedDtoTariff, result.getValue());
//    }

//    @Test
//    public void testGetAllTariffsByEidAdministratorId() {
//        UUID testId = UUID.randomUUID();
//
//        when(tariffService.getAllTariffsByEidManagerId(testId)).thenReturn(List.of(testEntityTariff));
//        when(tariffMapper.mapToEntityListDto(List.of(testEntityTariff))).thenReturn(List.of(expectedDtoTariff));
//
//        RemoteInvocationResult result = listenerDispatcher.getAllTariffsByEidManagerId(testId);
//
//        assertNotNull(result);
//        Assertions.assertEquals(List.of(expectedDtoTariff), result.getValue());
//
//        verify(tariffService).getAllTariffsByEidManagerId(testId);
//        verify(tariffMapper).mapToEntityListDto(List.of(testEntityTariff));
//        verifyNoMoreInteractions(tariffService, tariffMapper);
//    }

    @Test
    public void testCalculateTariff() {
        CalculateTariff calculateTariff = new CalculateTariff(true);
        CalculateTariffResultDTO calculateTariffResultDTO = new CalculateTariffResultDTO();

        when(tariffService.calculateTariff(calculateTariff)).thenReturn(calculateTariffResultDTO);

        RemoteInvocationResult result = listenerDispatcher.calculateTariff(calculateTariff);

        assertNotNull(result);
        Assertions.assertEquals(calculateTariffResultDTO, result.getValue());

        verify(tariffService).calculateTariff(calculateTariff);
        verifyNoMoreInteractions(tariffService);
    }

//    @Test
//    public void testCreateDeviceTariff() {
//    	expectedDtoTariff = new TariffDTO(UUID.randomUUID(), LocalDate.now(), 10d, UUID.randomUUID(), UUID.randomUUID(), UUID.randomUUID(), CurrencyEnum.BGN);
//
//        when(tariffService.createTariff(expectedDtoTariff)).thenReturn(testEntityDeviceTariff);
//        when(tariffMapper.mapToTariffDto(testEntityDeviceTariff)).thenReturn(expectedDtoTariff);
//
//        RemoteInvocationResult result = listenerDispatcher.createTariff(expectedDtoTariff);
//
//        assertNotNull(result);
//        Assertions.assertEquals(expectedDtoTariff, result.getValue());
//    }

    @Test
    public void testGetDiscountByDateAndEidAdministratorId() {
        DeviceTariffDTO testDeviceTariffDTO = new DeviceTariffDTO();
        testDeviceTariffDTO.setEidAdministratorId(UUID.randomUUID());
        testDeviceTariffDTO.setStartDate(LocalDate.now());

        when(discountService.getDiscountByDateAndEidManagerId(expectedDtoDiscountDate)).thenReturn(testEntityDiscount);

        DiscountDateDTO testEntityDiscountDate = new DiscountDateDTO();
        testEntityDiscountDate.setDate(testEntityDiscountDate.getDate());
        testEntityDiscountDate.setEidManagerId(testEntityDiscountDate.getEidManagerId());

       RemoteInvocationResult result = listenerDispatcher.getDiscountByDateAndEidManagerId(testEntityDiscountDate);

        assertNotNull(result);
    }

    @Test
    public void testGetDiscountByDateAndEidAdministratorId_EntityNotFound() {
        DiscountDateDTO discountDateDTO = new DiscountDateDTO();
        discountDateDTO.setEidManagerId(UUID.randomUUID());
        discountDateDTO.setDate(LocalDate.now());
        String entityName = "Discount";

       when(discountService.getDiscountByDateAndEidManagerId(discountDateDTO))
                .thenThrow(new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, entityName, discountDateDTO.getEidManagerId().toString()));

        EntityNotFoundException exception = assertThrows(EntityNotFoundException.class, () -> {
            listenerDispatcher.getDiscountByDateAndEidManagerId(discountDateDTO);
        });

        String expectedMessage = String.format(ErrorCode.ENTITY_NOT_FOUND.getDetail(), entityName, discountDateDTO.getEidManagerId().toString());

        String actualMessage = exception.getMessage();
        assertTrue(actualMessage.contains(expectedMessage), "Actual message does not match the expected error message");

        verify(discountService).getDiscountByDateAndEidManagerId(discountDateDTO);
        verifyNoMoreInteractions(discountService);
    }

    @Test
    public void testGetAllDiscountsByEidAdministratorId() {
        UUID testId = UUID.randomUUID();
        when(discountService.getAllDiscountsByEidManagerId(testId)).thenReturn(List.of(testEntityDiscount));

        RemoteInvocationResult result = listenerDispatcher.getAllDiscountsByEidManagerId(testId);

        assertNotNull(result);
    }

//    @Test
//    public void testCreateDiscount() {
//        when(discountService.createDiscount(expectedDtoDiscount)).thenReturn(testEntityDiscount);
//
//        RemoteInvocationResult result = listenerDispatcher.createDiscount(expectedDtoDiscount);
//
//        assertNotNull(result);
//    }

}
