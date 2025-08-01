/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.certificates.all

import android.os.Parcelable
import androidx.annotation.ColorRes
import androidx.annotation.DrawableRes
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class CertificatesStatusEnum(
    override val type: String,
    val serverValue: String?,
    val title: StringSource,
    @param:ColorRes val colorRes: Int,
    @param:DrawableRes val iconRes: Int?,
) : TypeEnum, CommonListElementIdentifier, Parcelable {
    UNKNOWN(
        type = "",
        serverValue = null,
        title = StringSource(R.string.unknown),
        colorRes = R.color.color_BF1212,
        iconRes = null
    ),
    ALL(
        type = "ALL",
        serverValue = null,
        title = StringSource(R.string.all),
        colorRes = R.color.color_018930,
        iconRes = null
    ),
    ACTIVE(
        type = "ACTIVE",
        serverValue = "ACTIVE",
        title = StringSource(R.string.certificate_statuses_enum_active_title),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check
    ),
    INVALID(
        type = "INVALID",
        serverValue = "INVALID",
        title = StringSource(R.string.certificate_statuses_enum_invalid_title),
        colorRes = R.color.color_BF1212,
        iconRes = R.drawable.ic_cancel
    ),
    STOPPED(
        type = "STOPPED",
        serverValue = "STOPPED",
        title = StringSource(R.string.certificate_statuses_enum_stopped_title),
        colorRes = R.color.color_F59E0B,
        iconRes = R.drawable.ic_pause
    ),
    SIGNED(
        type = "SIGNED",
        serverValue = "SIGNED",
        title = StringSource(R.string.certificate_statuses_enum_signed_title),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check
    ),
    CREATED(
        type = "CREATED",
        serverValue = "CREATED",
        title = StringSource(R.string.certificate_statuses_enum_created_title),
        colorRes = R.color.color_4C6F9E,
        iconRes = R.drawable.ic_clock
    ),
    REVOKED(
        type = "REVOKED",
        serverValue = "REVOKED",
        title = StringSource(R.string.certificate_statuses_enum_revoked_title),
        colorRes = R.color.color_BF1212,
        iconRes = R.drawable.ic_erase
    ),
    EXPIRED(
        type = "EXPIRED",
        serverValue = "EXPIRED",
        title = StringSource(R.string.certificate_statuses_enum_expired_title),
        colorRes = R.color.color_94A3B8,
        iconRes = R.drawable.ic_retrieved
    ),
}