/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.all

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ApplicationDeviceTypeEnum(
    override val type: String,
    val title: StringSource,
) : TypeEnum, Parcelable {
    UNKNOWN("", StringSource(R.string.unknown)),
    MOBILE("MOBILE", StringSource(R.string.application_device_type_enum_mobile_application)),
    IDENTITY_CARD("IDENTITY_CARD", StringSource(R.string.application_device_type_enum_identity_card)),
}