package bg.bulsi.mvr.iscei.contract.dto.approvalrequest;

public record RelyPartyRequest (String login_hint, String scope, boolean is_consent_required, String binding_message, String acr_values) {

}
