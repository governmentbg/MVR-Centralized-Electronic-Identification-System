package com.digitall.eid.models.common.validator

import android.annotation.SuppressLint
import android.icu.text.SimpleDateFormat
import androidx.core.text.isDigitsOnly
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource

class PersonalIdentifierValidator(override val errorMessage: StringSource = StringSource(R.string.unknown)) :
    Validator<String?> {

    override fun validate(value: String?): ValidationResult {
        if (value.isNullOrBlank()) {
            return ValidationResult.Success
        }

        if (value.length < 10 || value.isDigitsOnly().not()) {
            return ValidationResult.Error(message = StringSource(R.string.error_invalid_personal_identifier_format))
        }

        val isPotentiallyEGN = isValidEGN(value)
        val isPotentiallyLNCH = isValidLNCH(value)

        return when {
            isPotentiallyEGN -> ValidationResult.Success
            isPotentiallyLNCH -> ValidationResult.Success
            else -> ValidationResult.Error(message = StringSource(R.string.error_invalid_personal_identifier))
        }
    }


    private fun isValidLNCH(lnch: String): Boolean {
        val lastDigit = calculateLastDigit(lnch)
        return isValidLastDigit(lastDigit, lnch)
    }

    private fun calculateLastDigit(lnch: String): Int {
        val weights = listOf(21, 19, 17, 13, 11, 9, 7, 3, 1)
        var lastDigit =
            weights.indices.sumOf { weights[it] * (lnch[it].toString().toIntOrNull() ?: 0) }
        lastDigit %= 10
        return lastDigit
    }

    private fun isValidLastDigit(lastDigit: Int, egnOrLnch: String): Boolean {
        return lastDigit == (egnOrLnch.substring(9, 10).toIntOrNull() ?: 0)
    }

    private fun isValidEGN(
        personalIdentificationNumber: String,
    ): Boolean {
        return checkValidationCode(personalIdentificationNumber)
                && checkValidDate(personalIdentificationNumber)
    }

    private fun checkValidationCode(personalIdentificationNumber: String): Boolean {
        return try {
            val weight = intArrayOf(2, 4, 8, 5, 10, 9, 7, 3, 6)
            val mySum = weight.indices.sumOf {
                weight[it] * (personalIdentificationNumber[it].toString().toIntOrNull() ?: 0)
            }
            personalIdentificationNumber.last().toString() == (mySum % 11).toString().last()
                .toString()
        } catch (e: NumberFormatException) {
            logError("checkValidationCode Exception: ${e.message}", e, "checkValidationCode")
            false
        }
    }

    @SuppressLint("SimpleDateFormat")
    private fun checkValidDate(personalIdentificationNumber: String): Boolean {
        try {
            val year = personalIdentificationNumber.substring(0, 2).toInt()
            val month = personalIdentificationNumber.substring(2, 4).toInt()
            val day = personalIdentificationNumber.substring(4, 6).toInt()
            val adjustedYear: Int = when {
                month >= 40 -> year + 2000
                month >= 20 -> year + 1800
                else -> year + 1900
            }
            val monthString = if (month > 9) month.toString() else "0$month"
            val dayString = if (day > 9) day.toString() else "0$day"
            val dateString = "$adjustedYear-$monthString-$dayString"
            val dateFormat = SimpleDateFormat("yyyy-MM-dd")
            dateFormat.isLenient = false
            dateFormat.parse(dateString)
            return true
        } catch (e: Exception) {
            logError("checkValidDate Exception: ${e.message}", e, "checkValidationCode")
            return false
        }
    }
}