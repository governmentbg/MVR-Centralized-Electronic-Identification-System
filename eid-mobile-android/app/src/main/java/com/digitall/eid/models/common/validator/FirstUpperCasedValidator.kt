package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource

class FirstUpperCasedValidator(
    override val errorMessage: StringSource = StringSource(
        R.string.error_names_uppercase
    )
) : Validator<String?> {

    override fun validate(value: String?): ValidationResult {
        if (value.isNullOrBlank()) return ValidationResult.Success

        return if (value.firstOrNull()?.isUpperCase() == true) ValidationResult.Success
            else ValidationResult.Error(message = errorMessage)
    }
}