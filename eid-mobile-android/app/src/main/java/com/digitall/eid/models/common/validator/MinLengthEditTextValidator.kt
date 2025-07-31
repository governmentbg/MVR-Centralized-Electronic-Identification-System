package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource

class MinLengthEditTextValidator(
    private val minLength: Int,
    override val errorMessage: StringSource = StringSource(
        R.string.error_field_symbols_length, listOf(minLength.toString())
    )
) : Validator<String?> {
    override fun validate(value: String?): ValidationResult {
        if (value.isNullOrBlank()) return ValidationResult.Success

        return if (value.length >= minLength) ValidationResult.Success
            else ValidationResult.Error(message = errorMessage)
    }
}