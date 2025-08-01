package bg.bulsi.mvr.mpozei.contract.dto;

import lombok.Data;

import java.io.Serializable;
import java.time.OffsetDateTime;
import java.util.UUID;

import org.springframework.format.annotation.DateTimeFormat;

@Data
public class RueiPublicCertificateInfo implements Serializable {
        private String commonName;
        private UUID deviceId;
        private CertificateStatus status;
        private OffsetDateTime validityFrom;
        private OffsetDateTime validityUntil;
        private String serialNumber;
}
