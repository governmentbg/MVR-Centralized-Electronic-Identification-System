/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.details

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource

const val ONLINE_OFFICE = "ONLINE"

enum class ApplicationDetailsTypeEnum(
    override val type: String,
    val title: StringSource,
) : TypeEnum {
    UNKNOWN("", StringSource(R.string.unknown)),
    ISSUE_EID("ISSUE_EID", StringSource(R.string.application_details_enum_type_issue)),
    RESUME_EID("RESUME_EID", StringSource(R.string.application_details_enum_type_resume)),
    REVOKE_EID("REVOKE_EID", StringSource(R.string.application_details_enum_type_revoke)),
    STOP_EID("STOP_EID", StringSource(R.string.application_details_enum_type_stop)),
}