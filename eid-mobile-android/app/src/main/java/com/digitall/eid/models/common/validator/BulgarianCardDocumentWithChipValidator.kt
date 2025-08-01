package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.extensions.isLatin
import com.digitall.eid.models.common.StringSource

class BulgarianCardDocumentWithChipValidator(
    override val errorMessage: StringSource = StringSource(
        R.string.error_card_chip_unsupported
    )
) : Validator<String?> {

    override fun validate(value: String?): ValidationResult {
        return if (value?.count { it.isDigit() } == 7 && value.count { it.isLatin() } == 2) ValidationResult.Success
        else ValidationResult.Error(message = errorMessage)
    }
}