/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.database.converters

import androidx.room.TypeConverter
import java.util.Date

class DateToLongConverter {

    @TypeConverter
    fun toLong(date: Date?): Long? {
        if (date == null) return null
        return date.time
    }

    @TypeConverter
    fun fromLong(date: Long?): Date? {
        if (date == null) return null
        return Date(date)
    }

}