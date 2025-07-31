package com.digitall.eid.models.common.validator

import com.digitall.eid.models.common.StringSource

interface Validator<T> {
    /**
     * Validates the given value.
     *
     * @param value The value to validate.
     * @return A [ValidationResult] indicating success or failure with an error message.
     */
    fun validate(value: T): ValidationResult

    val errorMessage: StringSource
}