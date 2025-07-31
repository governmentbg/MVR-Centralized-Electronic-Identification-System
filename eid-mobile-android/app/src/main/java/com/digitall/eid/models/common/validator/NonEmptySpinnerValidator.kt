package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi

class NonEmptySpinnerValidator (override val errorMessage: StringSource = StringSource(R.string.error_field_required)) :
    Validator<CommonSpinnerMenuItemUi?> {

    override fun validate(value: CommonSpinnerMenuItemUi?): ValidationResult {
        return if (value != null) ValidationResult.Success else ValidationResult.Error(message = errorMessage)
    }
}