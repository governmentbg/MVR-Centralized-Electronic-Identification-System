package bg.bulsi.mvr.mpozei.backend.client;

import bg.bulsi.mvr.mpozei.backend.client.config.FeignInternalConfig;
import bg.bulsi.mvr.mpozei.backend.dto.*;
import bg.bulsi.mvr.mpozei.backend.dto.mock.MockCitizenCertificateDTO;
import bg.bulsi.mvr.mpozei.backend.dto.mock.MockCitizenCertificateValidateDTO;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import org.springdoc.api.annotations.ParameterObject;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;
import java.util.UUID;

@FeignClient(name = "ruei-client", url = "${services.ruei-gateway-base-url}", configuration = FeignInternalConfig.class)
public interface RueiClient {
    @PostMapping("/api/v1/certificate-registry/certificates")
    UUID createCitizenCertificate(CitizenCertificateDTO dto);

//    @PostMapping("/api/v1/certificate-registry/certificates/mocked")
//    UUID createMockedCitizenCertificate(MockCitizenCertificateDTO dto);

    @PostMapping("/api/v1/certificate-registry/citizens/attach")
    CitizenProfileDTO attachCitizenProfile(CitizenProfileAttachDTO dto);

    @PostMapping("/api/v1/certificate-registry/certificates/validate")
    CertificateStatusDTO validateCitizenCertificate(CitizenCertificateValidateDTO dto);

//    @PostMapping("/api/v1/certificate-registry/certificates/validate/mocked")
//    CertificateStatusDTO validateMockCitizenCertificate(MockCitizenCertificateValidateDTO dto);

    @PutMapping("/api/v1/certificate-registry/certificates")
    CertificateStatusDTO updateCertificateStatus(CitizenCertificateUpdateDTO dto);

    @PutMapping("/api/v1/certificate-registry/naif/certificates/status")
    CitizenCertificateDetailsDTO updateCertificateStatusByNaif(RueiUpdateCertificateStatusNaifDTO dto);

    @GetMapping("/api/v1/certificate-registry/certificates/{certificateId}/summary")
    CitizenCertificateSummaryDTO getCertificateById(@PathVariable UUID certificateId);

    @GetMapping("/api/v1/certificate-registry/eidentities/{eidentityId}/citizen-profile")
    CitizenProfileDTO getCitizenProfileByEidentityId(@PathVariable UUID eidentityId);

    @GetMapping("/api/v1/certificate-registry/citizens/profile/{citizenProfileId}")
    CitizenProfileDTO getCitizenProfileById(@PathVariable UUID citizenProfileId);

    @GetMapping("/api/v1/certificate-registry/citizens/profile")
    CitizenProfileDTO getCitizenProfileByEmail(@RequestParam String email);
    
    @PostMapping("/api/v1/certificate-registry/citizens/profile-verification")
    void validateCitizenProfile(@RequestBody RueiVerifyProfileDTO request);

    @GetMapping("/api/v1/certificate-registry/certificates")
    CitizenCertificateDetailsDTO getCitizenCertificateByIssuerAndSN(@RequestParam String issuer, @RequestParam String serialNumber);

    @GetMapping("/api/v1/certificate-registry/certificates/find")
    Page<FindCertificateResponse> findCitizenCertificates(@RequestParam UUID eidentityId,
                                                          @RequestParam UUID citizenProfileId,
                                                          @RequestParam String id,
                                                          @RequestParam String serialNumber,
                                                          @RequestParam List<CertificateStatus> status,
                                                          @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate validityFrom,
                                                          @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate validityUntil,
                                                          @RequestParam List<UUID> deviceIds,
                                                          @RequestParam String alias,
                                                          @RequestParam boolean publicApi,
                                                          @ParameterObject Pageable pageable);

    @PostMapping("/api/v1/certificate-registry/citizens/register")
    String registerCitizenProfile(@RequestBody CitizenProfileRegistrationDTO dto);

    @GetMapping("/api/v1/certificate-registry/citizens/register")
    String verifyCitizenProfile(@RequestParam UUID token);

    @PutMapping("/api/v1/certificate-registry/citizens/{id}/profile")
    String updateCitizenProfile(@PathVariable UUID id, @RequestBody CitizenProfileUpdateRequest request);

    @PostMapping(value = "/api/v1/certificate-registry/citizens/forgotten-password")
    String forgottenPassword(@RequestBody ForgottenPasswordDTO dto);

    @PostMapping(value = "/api/v1/certificate-registry/citizens/reset-password")
    String resetPassword(@RequestParam String token, @RequestParam String password);

    @PostMapping(value = "/api/v1/certificate-registry/citizens/update-password")
    String updateCitizenProfilePassword(@RequestParam String oldPassword, @RequestParam String newPassword);

    @PostMapping(value = "/api/v1/certificate-registry/citizens/update-email")
    HttpResponse updateCitizenProfileEmail(@RequestParam String email);

    @PostMapping(value = "/api/v1/certificate-registry/citizens/update-email/confirm")
    HttpResponse confirmUpdateCitizenProfileEmail(@RequestParam UUID token);

    @PostMapping(value = "/api/v1/certificate-registry/certificates/invalidate-by-eidentityid")
    List<CitizenCertificateDetailsDTO> invalidateCertificatesByEidentityIds(@RequestParam List<UUID> eidentityIds);

    @PostMapping(value = "/api/v1/certificate-registry/certificates/invalidate-by-date")
    List<CitizenCertificateDetailsDTO> invalidateExpiredCertificates(@RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date);

    @PutMapping(value = "/api/v1/certificate-registry/profile/status")
    void updateProfileStatusByEidentityId(@RequestParam ProfileStatus status, UUID id);

    @PutMapping(value = "/api/v1/certificate-registry/certificate/alias")
    HttpResponse updateCitizenCertificateAlias(@RequestParam UUID certificateId, @RequestParam String alias);

    @GetMapping(value = "/api/v1/certificate-registry/certificate/public-find")
    Page<RueiPublicCertificateInfo> getPublicCertificateInfos(@RequestParam String keyword, @RequestParam Pageable pageable);
}
