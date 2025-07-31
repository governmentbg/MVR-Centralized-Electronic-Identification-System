package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource

class SharedNameDependencyValidator(
    private val primaryNameProvider: () -> String?,
    private val siblingNameProvider: () -> String?,
    override val errorMessage: StringSource = StringSource(R.string.error_names_combination)
) : Validator<String?> {

    override fun validate(value: String?): ValidationResult {
        val primaryName = primaryNameProvider()?.trim()

        if (primaryName.isNullOrBlank()) {
            return ValidationResult.Success
        }

        val currentFieldText = value?.trim()
        val siblingFieldText = siblingNameProvider()?.trim()

        return if (currentFieldText.isNullOrBlank().not() ||
            siblingFieldText.isNullOrBlank().not()
        ) ValidationResult.Success
        else ValidationResult.Error(message = errorMessage)
    }
}