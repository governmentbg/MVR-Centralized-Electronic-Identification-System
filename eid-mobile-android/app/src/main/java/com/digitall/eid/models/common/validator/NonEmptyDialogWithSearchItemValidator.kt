package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonDialogWithSearchItemUi

class NonEmptyDialogWithSearchItemValidator(override val errorMessage: StringSource = StringSource(R.string.error_field_required)) :
    Validator<CommonDialogWithSearchItemUi?> {

    override fun validate(value: CommonDialogWithSearchItemUi?): ValidationResult {
        return if (value != null) ValidationResult.Success else ValidationResult.Error(message = errorMessage)
    }
}