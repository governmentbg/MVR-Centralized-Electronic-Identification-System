package bg.bulsi.mvr.mpozei.backend.dto.ejbca;

public enum EjbcaRevocationReason {
    NOT_REVOKED,
    UNSPECIFIED,
    KEY_COMPROMISE, //the only allowed value for update on revocation reason
    CA_COMPROMISE,
    AFFILIATION_CHANGED,
    SUPERSEDED,
    CESSATION_OF_OPERATION,
    CERTIFICATE_HOLD,
    REMOVE_FROM_CRL,
    PRIVILEGES_WITHDRAWN,
    AA_COMPROMISE
}
