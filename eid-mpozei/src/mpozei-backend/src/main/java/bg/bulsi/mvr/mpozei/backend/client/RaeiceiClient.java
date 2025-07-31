package bg.bulsi.mvr.mpozei.backend.client;

import bg.bulsi.mvr.mpozei.backend.client.config.FeignInternalConfig;
import bg.bulsi.mvr.mpozei.backend.dto.CalculateTariffRequest;
import bg.bulsi.mvr.mpozei.backend.dto.CalculateTariffResponse;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceResponseDTO;
import bg.bulsi.mvr.raeicei.model.enums.Region;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;
import java.util.UUID;

@FeignClient(name = "raeicei-client", url = "${services.raeicei-gateway-base-url}", configuration = FeignInternalConfig.class)
public interface RaeiceiClient {
    @GetMapping("/raeicei/api/v1/eidmanagerfrontoffice")
    EidManagerFrontOfficeDTO getOfficeByName(@RequestParam String name);

    @GetMapping("/raeicei/api/v1/eidmanagerfrontoffice/{id}")
    EidManagerFrontOfficeDTO getOfficeById(@PathVariable UUID id);

    @GetMapping("/raeicei/api/v1/eidadministrator/{id}")
    EidAdministratorDTO getEidAdministratorById(@PathVariable UUID id);

    @GetMapping("/raeicei/api/v1/device/{id}")
    DeviceDTO getDeviceById(@PathVariable UUID id);

    @GetMapping("/raeicei/api/v1/device")
    List<DeviceDTO> getDeviceByType(@RequestParam String type);

    @GetMapping("/raeicei/api/v1/eidmanagerfrontoffice/getAll/region/{region}")
    List<EidManagerFrontOfficeDTO> getAllOfficesByRegion(@PathVariable Region region, @RequestParam LocalDate from, @RequestParam LocalDate dtoTo);

    @PostMapping("/raeicei/api/v1/calculateTariff")
    CalculateTariffResponse calculateTariff(@RequestBody CalculateTariffRequest request);

    @GetMapping("/raeicei/api/v1/providedservice/get/applicationType/{applicationType}")
    ProvidedServiceResponseDTO getProvidedServiceByApplicationType(@RequestParam ApplicationType applicationType);
}
