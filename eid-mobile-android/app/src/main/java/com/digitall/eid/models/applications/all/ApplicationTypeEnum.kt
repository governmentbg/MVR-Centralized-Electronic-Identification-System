/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.all

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ApplicationTypeEnum(
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
        StringSource(R.string.application_action_type_issue)
    ),
    RESUME_EID(
        "RESUME_EID",
        "RESUME_EID",
        StringSource(R.string.application_action_type_resume)
    ),
    REVOKE_EID(
        "REVOKE_EID",
        "REVOKE_EID",
        StringSource(R.string.application_action_type_revoke)
    ),
    STOP_EID(
        "STOP_EID",
        "STOP_EID",
        StringSource(R.string.application_action_type_stop)
    ),
}