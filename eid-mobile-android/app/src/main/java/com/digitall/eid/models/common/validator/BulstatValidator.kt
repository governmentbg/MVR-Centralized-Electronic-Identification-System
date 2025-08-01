package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource

class BulstatValidator(override val errorMessage: StringSource = StringSource(R.string.error_invalid_bulstat)) :
    Validator<String?> {

    override fun validate(value: String?): ValidationResult {
        if (value.isNullOrBlank()) {
            return ValidationResult.Success
        }

        return if (listOf(
                9,
                13
            ).contains(value.length)
        ) ValidationResult.Success else ValidationResult.Error(errorMessage)
    }
}