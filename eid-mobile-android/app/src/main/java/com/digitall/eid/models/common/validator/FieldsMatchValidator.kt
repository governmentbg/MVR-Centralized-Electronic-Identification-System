package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource

class FieldsMatchValidator(
    private val originalFieldTextProvider: () -> String?,
    override val errorMessage: StringSource = StringSource(R.string.error_password_mismatch)
) : Validator<String?> {

    override fun validate(value: String?): ValidationResult {
        val originalText = originalFieldTextProvider()
        val confirmText = value ?: ""

        return if (originalText == confirmText) ValidationResult.Success else ValidationResult.Error(
            message = errorMessage
        )
    }
}