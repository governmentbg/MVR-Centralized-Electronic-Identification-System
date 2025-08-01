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
enum class CreatePinErrorsEnum(
    override val type: String,
    val title: StringSource,
) : TypeEnum, Parcelable {
    PASS_CODE_IS_SEQUENT_DIGITS(
        type = "PASSWORD_CONTAINS_SEQUENTIAL_DIGITS",
        title = StringSource(R.string.create_pin_error_pass_code_cannot_be_set_as_sequential_numbers),
    ),
    PASS_CODE_HAS_REPEATED_DIGITS(
        type = "PASSWORD_CONTAINS_REPEATED_DIGITS",
        title = StringSource(R.string.create_pin_error_pass_code_repeating_digits_more_than_2),
    ),
    PASS_CODE_HAS_DATE_OF_BIRTH(
        type = "PASSWORD_CONTAINS_BIRTHDAY",
        title = StringSource(R.string.create_pin_error_pass_code_your_date_of_birth),
    ),
}