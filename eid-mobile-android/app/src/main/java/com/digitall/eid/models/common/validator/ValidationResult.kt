package com.digitall.eid.models.common.validator

import com.digitall.eid.models.common.StringSource

sealed class ValidationResult {
    data object Success : ValidationResult()
    data class Error(val message: StringSource) : ValidationResult()
}