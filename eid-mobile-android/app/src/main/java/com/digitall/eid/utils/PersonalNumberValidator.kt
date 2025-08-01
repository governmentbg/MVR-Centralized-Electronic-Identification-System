/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.utils

import android.annotation.SuppressLint
import android.icu.text.SimpleDateFormat
import androidx.core.text.isDigitsOnly
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource

fun isValidLNCH(lnch: String): Boolean {
    if (!lnch.isDigitsOnly()) return false
    if (!isValidLength(lnch)) return false
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

private fun isValidLength(egnOrLnch: String): Boolean {
    return egnOrLnch.length == 10
}

fun isValidEGN(
    personalIdentificationNumber: String,
): Boolean {
    return personalIdentificationNumber.length == 10
            && checkValidationCode(personalIdentificationNumber)
            && checkValidDate(personalIdentificationNumber)
}

private fun checkValidationCode(personalIdentificationNumber: String): Boolean {
    return try {
        val weight = intArrayOf(2, 4, 8, 5, 10, 9, 7, 3, 6)
        val mySum = weight.indices.sumOf {
            weight[it] * personalIdentificationNumber[it].toString().toInt()
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

fun translateUidType(
    uidType: String?,
    uidEmpowerer: String?,
    nameEmpowerer: String?,
) = when (uidType) {
    "EGN", "2" -> nameEmpowerer.takeIf { it.isNullOrEmpty().not() }?.let { name ->
        StringSource(
            R.string.empowerment_from_me_type_egn_and_name,
            listOf(uidEmpowerer ?: "", name)
        )
    } ?: StringSource(R.string.empowerment_from_me_type_egn, listOf(uidEmpowerer ?: ""))

    "LNCH", "LNCh", "1" -> nameEmpowerer.takeIf { it.isNullOrEmpty().not() }?.let { name ->
        StringSource(
            R.string.empowerment_from_me_type_lnch_and_name,
            listOf(uidEmpowerer ?: "", name)
        )
    } ?: StringSource(R.string.empowerment_from_me_type_lnch, listOf(uidEmpowerer ?: ""))

    "0" -> StringSource(R.string.not_specified)
    null, "" -> StringSource(R.string.unknown)
    else -> StringSource(uidType)
}