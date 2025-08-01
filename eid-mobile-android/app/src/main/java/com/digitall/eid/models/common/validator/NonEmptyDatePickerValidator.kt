package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource
import java.util.Calendar

class NonEmptyDatePickerValidator(override val errorMessage: StringSource = StringSource(R.string.error_field_required)) :
    Validator<Calendar?> {

    override fun validate(value: Calendar?): ValidationResult {
        return if (value != null) ValidationResult.Success else ValidationResult.Error(message = errorMessage)
    }
}