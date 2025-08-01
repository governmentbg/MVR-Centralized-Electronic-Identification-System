package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource

class NonEmptyEditTextValidator(override val errorMessage: StringSource = StringSource(R.string.error_field_required)) :
    Validator<String?> {
    override fun validate(value: String?): ValidationResult {
        return if (value.isNullOrBlank().not()) ValidationResult.Success else ValidationResult.Error(message = errorMessage)
    }
}