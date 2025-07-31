package bg.bulsi.mvr.mpozei.backend;

import bg.bulsi.mvr.mpozei.backend.pipeline.steps.*;
import lombok.AllArgsConstructor;
import lombok.Getter;
import org.springframework.stereotype.Component;

@Component
@AllArgsConstructor
@Getter
//TODO: use this later tp check if a step is being executed
public class PipelineSteps {
	private CertificateHistoryCreationStep certificateHistoryCreationStep;
	private EjbcaCertificateRetrievalStep ejbcaCertificateRetrievalStep;
	private EjbcaCertificateRevocationStep ejbcaCertificateRevocationStep;
	private PivrVerificationStep pivrVerificationStep;
	private PunCarrierCreationStep punCarrierCreationStep;
	private RaiceiVerificationStep raiceiVerificationStep;
	private RegiXVerificationStep regiXVerificationStep;
	private ReiEidentityCreationStep reiEidentityCreationStep;
	private ReiEidentityVerificationStep reiEidentityVerificationStep;
	private RueiBaseProfileAttachmentStep rueiBaseProfileAttachmentStep;
	private RueiBaseProfileVerificationStep rueiBaseProfileVerificationStep;
	private SendNotificationStep sendNotificationStep;
	private SignApplicationStep signApplicationStep;
	private DetachedSignatureVerificationStep detachedSignatureVerificationStep;
}
