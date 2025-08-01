/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.database.converters

import androidx.room.TypeConverter
import java.math.BigDecimal

class BigDecimalToStringConverter {

    @TypeConverter
    fun toString(data: BigDecimal?): String? {
        if (data == null) return null
        return data.toPlainString()
    }

}