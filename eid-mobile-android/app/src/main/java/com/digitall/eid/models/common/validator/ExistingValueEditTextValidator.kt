package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource

class ExistingValueEditTextValidator(
    private val existingValuesProvider: () -> Collection<String?>,
    override val errorMessage: StringSource = StringSource(R.string.error_value_already_used)
) : Validator<String?> {

    override fun validate(value: String?): ValidationResult {
        if (value.isNullOrBlank()) {
            ValidationResult.Success
        }

        return if (existingValuesProvider.invoke().contains(value)
                .not()
        ) ValidationResult.Success else ValidationResult.Error(errorMessage)
    }
}