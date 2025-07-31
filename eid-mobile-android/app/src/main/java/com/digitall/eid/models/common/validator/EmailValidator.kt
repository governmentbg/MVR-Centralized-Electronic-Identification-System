package com.digitall.eid.models.common.validator

import com.digitall.eid.R
import com.digitall.eid.models.common.StringSource
import java.util.regex.Pattern

class EmailValidator(override val errorMessage: StringSource = StringSource(R.string.error_valid_email_format)) :
    Validator<String?> {

    private val emailPattern = Pattern.compile(
        "[A-Z0-9a-z._%+\\-]+@[A-Za-z0-9.\\-]+\\.[A-Za-z]{2,64}"
    )

    override fun validate(value: String?): ValidationResult {
        return if (emailPattern.matcher(value ?: "")
                .matches()
        ) ValidationResult.Success else ValidationResult.Error(message = errorMessage)
    }
}