package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectItemUi

class NonEmptyDialogWithSearchMultipleItemsValidator(override val errorMessage: StringSource = StringSource(
    R.string.error_field_required)
) :
    Validator<List<CommonDialogWithSearchMultiselectItemUi>?> {

    override fun validate(value: List<CommonDialogWithSearchMultiselectItemUi>?): ValidationResult {
        return if (value.isNullOrEmpty().not()) ValidationResult.Success else ValidationResult.Error(message = errorMessage)
    }
}