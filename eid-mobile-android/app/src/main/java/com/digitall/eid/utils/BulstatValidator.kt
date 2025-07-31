/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.utils

private val FIRST_SUM_9DIGIT_WEIGHTS = intArrayOf(1, 2, 3, 4, 5, 6, 7, 8)
private val SECOND_SUM_9DIGIT_WEIGHTS = intArrayOf(3, 4, 5, 6, 7, 8, 9, 10)
private val FIRST_SUM_13DIGIT_WEIGHTS = intArrayOf(2, 7, 3, 5)
private val SECOND_SUM_13DIGIT_WEIGHTS = intArrayOf(4, 9, 5, 7)

private fun safelyCalculateChecksumForNineDigitsEIK(eik: String): Boolean {
    return try {
        val digits = checkInput(eik, 9)
        val ninthDigit = calculateNinthDigitInEIK(digits)
        ninthDigit == digits[8]
    } catch (e: IllegalArgumentException) {
        false
    }
}

private fun safelyCalculateChecksumForThirteenDigitsEIK(eik: String): Boolean {
    return try {
        val digits = checkInput(eik, 13)
        val thirteenthDigit = calculateThirteenthDigitInEIK(digits)
        thirteenthDigit == digits[12]
    } catch (e: IllegalArgumentException) {
        false
    }
}

private fun calculateNinthDigitInEIK(digits: IntArray): Int {
    var sum = 0
    digits.toList().zip(FIRST_SUM_9DIGIT_WEIGHTS.toList()) { digit, weigth ->
        sum += digit * weigth
    }
    val remainder = sum % 11
    if (remainder != 10) {
        return remainder
    }
    var secondSum = 0
    digits.toList().zip(SECOND_SUM_9DIGIT_WEIGHTS.toList()) { digit, weigth ->
        secondSum += digit * weigth
    }
    val secondRem = secondSum % 11
    return if (secondRem != 10) secondRem else 0
}

private fun calculateThirteenthDigitInEIK(digits: IntArray): Int {
    val ninthDigit = calculateNinthDigitInEIK(digits)
    if (ninthDigit != digits[8]) {
        throw IllegalArgumentException("Incorrect 9th digit in EIK-13.")
    }
    var sum = 0
    val extraDigits = digits.slice(9 until digits.size)
    extraDigits.toList().zip(FIRST_SUM_13DIGIT_WEIGHTS.toList()) { digit, weigth ->
        sum += digit * weigth
    }
    val remainder = sum % 11
    if (remainder != 10) {
        return remainder
    }
    var secondSum = 0
    extraDigits.toList().zip(SECOND_SUM_13DIGIT_WEIGHTS.toList()) { digit, weigth ->
        secondSum += digit * weigth
    }
    val secondRem = secondSum % 11
    return if (secondRem != 10) secondRem else 0
}

private fun checkInput(eik: String, eikLength: Int): IntArray {
    require(eik.length == eikLength) {
        "Incorrect count of digits in EIK: ${eik.length} != $eikLength"
    }
    return eik.map { char ->
        char.digitToIntOrNull() ?: throw IllegalArgumentException("Incorrect input character. Only digits are allowed.")
    }.toIntArray()
}