package com.digitall.eid.models.payments.history.all

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class PaymentReasonEnum(
    override val type: String,
    val serverValue: String?,
    val title: StringSource,
) : TypeEnum, CommonListElementIdentifier {
    UNKNOWN(
        "",
        null,
        StringSource(R.string.unknown)
    ),
    ALL(
        "ALL",
        null,
        StringSource(R.string.all)
    ),
    ISSUE_EID(
        "ISSUE_EID",
        "ISSUE_EID",
        StringSource(R.string.payment_history_reason_enum_issueeid)
    ),
    RESUME_EID(
        "RESUME_EID",
        "RESUME_EID",
        StringSource(R.string.payment_history_reason_enum_resumeeid)
    ),
    REVOKE_EID(
        "REVOKE_EID",
        "REVOKE_EID",
        StringSource(R.string.payment_history_reason_enum_revokeeid)
    ),
    STOP_EID(
        "STOP_EID",
        "STOP_EID",
        StringSource(R.string.payment_history_reason_enum_stopeid)
    ),
}