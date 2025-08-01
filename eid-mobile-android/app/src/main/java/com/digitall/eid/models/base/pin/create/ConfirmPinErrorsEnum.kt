/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.base.pin.create

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ConfirmPinErrorsEnum(
    override val type: String,
    val title: StringSource,
) : TypeEnum, Parcelable {
    NOT_MATCH_WITH_ENTER(
        type = "NOT_MATCH_WITH_ENTER",
        title = StringSource(R.string.confirm_pin_error_not_match),
    ),
    HAS_REPETITION_OR_CONSECUTIVE_DIGITS(
        type = "HAS_REPETITION_OR_CONSECUTIVE_DIGITS",
        title = StringSource(R.string.confirm_pin_error_repetition_or_consecutive_digits),
    )
}