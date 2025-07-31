/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.all

import android.os.Parcelable
import androidx.annotation.ColorRes
import androidx.annotation.DrawableRes
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ApplicationStatusEnum(
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
    PAID(
        type = "PAID",
        serverValue = "PAID",
        title = StringSource(R.string.application_statuses_enum_paid_title),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check,
    ),
    DENIED(
        type = "DENIED",
        serverValue = "DENIED",
        title = StringSource(R.string.application_statuses_enum_denied_title),
        colorRes = R.color.color_BF1212,
        iconRes = R.drawable.ic_cancel,
    ),
    SIGNED(
        type = "SIGNED",
        serverValue = "SIGNED",
        title = StringSource(R.string.application_statuses_enum_signed_title),
        colorRes = R.color.color_4C6F9E,
        iconRes = R.drawable.ic_sign_blue
    ),
    APPROVED(
        type = "APPROVED",
        serverValue = "APPROVED",
        title = StringSource(R.string.application_statuses_enum_approved_title),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check,
    ),
    SUBMITTED(
        type = "SUBMITTED",
        serverValue = "SUBMITTED",
        title = StringSource(R.string.application_statuses_enum_submitted_title),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check,
    ),
    COMPLETED(
        type = "COMPLETED",
        serverValue = "COMPLETED",
        title = StringSource(R.string.application_statuses_enum_completed_title),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check,
    ),
    PROCESSING(
        type = "PROCESSING",
        serverValue = "PROCESSING",
        title = StringSource(R.string.application_statuses_enum_processing_title),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_warning,
    ),
    PENDING_PAYMENT(
        type = "PENDING_PAYMENT",
        serverValue = "PENDING_PAYMENT",
        title = StringSource(R.string.application_statuses_enum_pending_payment_title),
        colorRes = R.color.color_F59E0B,
        iconRes = R.drawable.ic_clock,
    ),
    PAYMENT_EXPIRED(
        type = "PAYMENT_EXPIRED",
        serverValue = "PAYMENT_EXPIRED",
        title = StringSource(R.string.application_statuses_enum_payment_expired_title),
        colorRes = R.color.color_BF1212,
        iconRes = R.drawable.ic_cancel,
    ),
    PENDING_SIGNATURE(
        type = "PENDING_SIGNATURE",
        serverValue = "PENDING_SIGNATURE",
        title = StringSource(R.string.application_statuses_enum_pending_signature_title),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_warning,
    ),
    CERTIFICATE_STORED(
        type = "CERTIFICATE_STORED",
        serverValue = "CERTIFICATE_STORED",
        title = StringSource(R.string.application_statuses_enum_stored_certificate_title),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check,
    ),
    GENERATED_CERTIFICATE(
        type = "GENERATED_CERTIFICATE",
        serverValue = "GENERATED_CERTIFICATE",
        title = StringSource(R.string.application_statuses_enum_generated_certificate_title),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check,
    ),
}