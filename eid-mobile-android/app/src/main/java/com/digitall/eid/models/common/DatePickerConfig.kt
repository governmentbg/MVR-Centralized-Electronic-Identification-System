/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.common

import com.digitall.eid.domain.extensions.getString
import java.util.Calendar

data class DatePickerConfig(
    val minDate: Calendar,
    val maxDate: Calendar,
    val selectedValue: Calendar,
) {

    override fun toString(): String {
        return "\nselectedValue: ${selectedValue.getString()}\nminDate: ${minDate.getString()}\nmaxDate: ${maxDate.getString()}\n"
    }

}