/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.extensions

import android.annotation.SuppressLint
import com.digitall.eid.domain.FromServerDateFormats
import com.digitall.eid.domain.TimeZones
import com.digitall.eid.domain.ToServerDateFormats
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.utils.LogUtil.logDebug
import java.text.SimpleDateFormat
import java.util.Calendar
import java.util.Date
import java.util.TimeZone

private const val TAG = "DateExtTag"

fun getCalendar(
    year: Int? = null,
    month: Int? = null,
    day: Int? = null,
    minusYears: Int? = null,
    plusYears: Int? = null,
    minusMonths: Int? = null,
    plusMonths: Int? = null,
    minusDays: Int? = null,
    plusDays: Int? = null,
    time: Date? = null,
    timeInMillis: Long? = null,
    timeZone: TimeZones = TimeZones.DEFAULT,
    setMidday: Boolean = false
): Calendar = Calendar.getInstance(TimeZone.getTimeZone(timeZone.type)).apply {
    logDebug(
        "getCalendar year: $year month: $month day: $day time: $time timeInMillis: $timeInMillis timeZone: $timeZone setMidday: $setMidday",
        TAG
    )
    if (time != null) {
        this.time = time
    }
    if (timeInMillis != null) {
        this.timeInMillis = timeInMillis
    }
    if (year != null) {
        set(Calendar.YEAR, year)
    }
    if (month != null) {
        set(Calendar.MONTH, month)
    }
    if (day != null) {
        set(Calendar.DAY_OF_MONTH, day)
    }
    if (minusYears != null) {
        add(Calendar.YEAR, -minusYears)
    }
    if (plusYears != null) {
        add(Calendar.YEAR, plusYears)
    }
    if (minusMonths != null) {
        add(Calendar.MONTH, -minusMonths)
        add(Calendar.DAY_OF_MONTH, -1)
    }
    if (plusMonths != null) {
        add(Calendar.MONTH, plusMonths)
        add(Calendar.DAY_OF_MONTH, -1)
    }
    if (minusDays != null) {
        add(Calendar.DAY_OF_MONTH, -minusDays)
    }
    if (plusDays != null) {
        add(Calendar.DAY_OF_MONTH, plusDays)
    }
    if (setMidday) {
        set(Calendar.HOUR_OF_DAY, 12)
        set(Calendar.MINUTE, 0)
        set(Calendar.SECOND, 0)
        set(Calendar.MILLISECOND, 0)
    }
}

@SuppressLint("SimpleDateFormat")
fun getSimpleDateFormat(
    format: String,
    timeZone: TimeZones = TimeZones.UTC,
) = SimpleDateFormat(format).apply {
    this.timeZone = TimeZone.getTimeZone(timeZone.type)
}

fun Calendar.setMidday() = apply {
    set(Calendar.HOUR_OF_DAY, 12)
    set(Calendar.MINUTE, 0)
    set(Calendar.SECOND, 0)
    set(Calendar.MILLISECOND, 0)
}

fun Calendar.atStartOfDay() = apply {
    set(Calendar.HOUR_OF_DAY, 0)
    set(Calendar.MINUTE, 0)
    set(Calendar.SECOND, 0)
    set(Calendar.MILLISECOND, 0)
}

fun Calendar.atEndOfDay() = apply {
    set(Calendar.HOUR_OF_DAY, 23)
    set(Calendar.MINUTE, 59)
    set(Calendar.SECOND, 59)
    set(Calendar.MILLISECOND, 999)
}

fun Calendar.getYear() = get(Calendar.YEAR)

fun Calendar.getMonth() = get(Calendar.MONTH)

fun Calendar.getDay() = get(Calendar.DAY_OF_MONTH)

fun Calendar.getHours() = get(Calendar.HOUR_OF_DAY)

fun Calendar.getMinutes() = get(Calendar.MINUTE)

fun Calendar.getSeconds() = get(Calendar.SECOND)

fun getMinCalendar() = getCalendar(
    year = 1900,
    month = 1,
    day = 1,
    setMidday = true,
)

fun Calendar.minusYears(
    minusYears: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = timeInMillis,
    minusYears = minusYears,
    setMidday = setMidday,
)

fun Calendar.plusYears(
    plusYears: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = timeInMillis,
    plusYears = plusYears,
    setMidday = setMidday,
)

fun Calendar.minusMonths(
    minusMonths: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = timeInMillis,
    minusMonths = minusMonths,
    setMidday = setMidday,
)

fun Calendar.plusMonths(
    plusMonths: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = timeInMillis,
    plusMonths = plusMonths,
    setMidday = setMidday,
)

fun Calendar.minusDays(
    minusDays: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = timeInMillis,
    minusDays = minusDays,
    setMidday = setMidday,
)

fun Calendar.plusDays(
    plusDays: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = timeInMillis,
    plusDays = plusDays,
    setMidday = setMidday,
)

fun Long.minusYears(
    minusYears: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = this,
    minusYears = minusYears,
    setMidday = setMidday,
)

fun Long.plusYears(
    plusYears: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = this,
    plusYears = plusYears,
    setMidday = setMidday,
)

fun Long.minusMonths(
    minusMonths: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = this,
    minusMonths = minusMonths,
    setMidday = setMidday,
)

fun Long.plusMonths(
    plusMonths: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = this,
    plusMonths = plusMonths,
    setMidday = setMidday,
)

fun Long.minusDays(
    minusDays: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = this,
    minusDays = minusDays,
    setMidday = setMidday,
)

fun Long.plusDays(
    plusDays: Int,
    setMidday: Boolean = true,
) = getCalendar(
    timeInMillis = this,
    plusDays = plusDays,
    setMidday = setMidday,
)

fun Long.toServerDate(
    dateFormat: ToServerDateFormats,
): String = getSimpleDateFormat(dateFormat.type).run {
    logDebug("Long.toServerDate", TAG)
    format(Date(this@toServerDate))
}

fun Calendar.toServerDate(
    dateFormat: ToServerDateFormats,
    timeZone: TimeZones = TimeZones.UTC,
): String = getSimpleDateFormat(dateFormat.type, timeZone).run {
    logDebug("Calendar.toServerDate", TAG)
    format(Date(this@toServerDate.timeInMillis))
}

fun String.fromServerDateToUnitTime(): Long? {
    logDebug("String.fromServerDateToUnitTime", TAG)
    FromServerDateFormats.entries.forEach { formatEnum ->
        try {
            getSimpleDateFormat(formatEnum.type).run {
                parse(this@fromServerDateToUnitTime)?.let {
                    return getCalendar(
                        time = it
                    ).timeInMillis
                }
            }
        } catch (e: Exception) {
            // try again
        }
    }
    return null
}

fun String.fromServerDate(timeZone: TimeZones = TimeZones.DEFAULT): Calendar? {
    logDebug("String.fromServerDate", TAG)
    FromServerDateFormats.entries.forEach { formatEnum ->
        try {
            getSimpleDateFormat(formatEnum.type).run {
                parse(this@fromServerDate)?.let {
                    return getCalendar(
                        time = it,
                        timeZone = timeZone,
                    )
                }
            }
        } catch (e: Exception) {
            // try again
        }
    }
    return null
}

fun Long.toTextDate(
    dateFormat: UiDateFormats,
    timeZone: TimeZones = TimeZones.DEFAULT
): String {
    logDebug("Long.toTextDate", TAG)
    return getSimpleDateFormat(dateFormat.type, timeZone).run {
        format(Date(this@toTextDate))
    }
}

fun Calendar.toTextDate(
    dateFormat: UiDateFormats,
    timeZone: TimeZones = TimeZones.DEFAULT
): String = getSimpleDateFormat(dateFormat.type, timeZone).run {
    logDebug("Calendar.toTextDate", TAG)
    format(Date(this@toTextDate.timeInMillis))
}

fun String.fromServerDateToTextDate(
    dateFormat: UiDateFormats,
    timeZone: TimeZones = TimeZones.DEFAULT
): String? {
    logDebug("fromServerDateToTextDate dateFormat: $dateFormat", TAG)
    FromServerDateFormats.entries.forEach { formatEnum ->
        try {
            getSimpleDateFormat(formatEnum.type, TimeZones.UTC).run {
                parse(this@fromServerDateToTextDate)?.let {
                    return getSimpleDateFormat(dateFormat.type, timeZone).format(it)
                }
            }
        } catch (e: Exception) {
            // try again
        }
    }
    return null
}

fun Calendar.getString(timeZone: TimeZones = TimeZones.DEFAULT): String =
    getSimpleDateFormat(UiDateFormats.WITH_TIME.type, timeZone).run {
        format(Date(this@getString.timeInMillis))
    }