package com.digitall.eid.models.empowerment.common.details

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class EmpowermentDetailsStatementsElementsUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum {
    NONE("None", StringSource(R.string.empowerment_details_statement_statuses_none_status_title)),
    ACTIVE("Active", StringSource(R.string.empowerment_details_statement_statuses_active_status_title)),
    CREATED("Created", StringSource(R.string.empowerment_details_statement_statuses_created_status_title)),
    DENIED("Denied", StringSource(R.string.empowerment_details_statement_statuses_denied_status_title)),
    DISAGREEMENT_DECLARED("DisagreementDeclared", StringSource(R.string.empowerment_details_statement_statuses_disagreement_declared_status_title)),
    WITHDRAWN("Withdrawn", StringSource(R.string.empowerment_details_statement_statuses_withdrawn_status_title)),
    COLLECTING_AUTHORIZER_SIGNATURES(
        type = "CollectingAuthorizerSignatures",
        title = StringSource(R.string.empowerment_details_statement_statuses_collecting_authorizer_signatures_status_title),
    ),
    COLLECTING_WITHDRAWAL_SIGNATURES(
        type = "CollectingWithdrawalSignatures",
        title = StringSource(R.string.empowerment_details_statement_statuses_collecting_withdrawal_signatures_status_title),
    ),
    AWAITING_SIGNATURE(
        type = "AwaitingSignature",
        title = StringSource(R.string.empowerments_entity_statuses_awaiting_signature_status_title),
    ),
    UNCONFIRMED("Unconfirmed", StringSource(R.string.empowerment_details_statement_statuses_unconfirmed_status_title));
}