package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource
import java.util.regex.Pattern

class PasswordValidator(override val errorMessage: StringSource = StringSource(R.string.error_valid_password_format)) :
    Validator<String?> {
    private val passwordPattern = Pattern.compile(
        "^(?=.*[0-9])(?=.*\\p{Lu})(?=.*\\p{Ll})(?=.*[!-/:-@-`{-~])(?=\\S+$).{8,}$"
    )

    override fun validate(value: String?): ValidationResult {
        return if (passwordPattern.matcher(value ?: "")
                .matches()
        ) ValidationResult.Success else ValidationResult.Error(message = errorMessage)
    }
}