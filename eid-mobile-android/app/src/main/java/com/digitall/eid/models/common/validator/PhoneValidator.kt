package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.domain.BG_COUNTRY_CODE
import com.digitall.eid.models.common.StringSource
import java.util.regex.Pattern

class PhoneValidator(override val errorMessage: StringSource = StringSource(R.string.error_valid_phone_number)) :
    Validator<String?> {

    private val bgPhonePattern = Pattern.compile(
        "^(\\+359)+(8[789])+\\d{7}"
    )

    override fun validate(value: String?): ValidationResult {
        return when {
            value.isNullOrEmpty() -> ValidationResult.Success
            else -> validatePhoneNumber(value)
        }
    }

    private fun validatePhoneNumber(value: String): ValidationResult {
        return if (bgPhonePattern.matcher(BG_COUNTRY_CODE + value)
                .matches()
        ) ValidationResult.Success
        else ValidationResult.Error(errorMessage)
    }
}