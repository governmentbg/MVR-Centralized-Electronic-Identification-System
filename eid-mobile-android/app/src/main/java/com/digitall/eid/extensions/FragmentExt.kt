/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

import android.app.DatePickerDialog
import androidx.fragment.app.Fragment
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.getDay
import com.digitall.eid.domain.extensions.getMonth
import com.digitall.eid.domain.extensions.getString
import com.digitall.eid.domain.extensions.getYear
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.common.DatePickerConfig
import java.util.Calendar

private const val TAG = "FragmentExtTag"

fun Fragment.showDataPicker(
    datePickerConfig: DatePickerConfig,
    dateSelected: (Calendar?) -> Unit,
) {
    logDebug("showDataPicker datePickerConfig: $datePickerConfig", TAG)
    val onDateSetListener = DatePickerDialog.OnDateSetListener { _, year, month, day ->
        val selected = getCalendar(
            year = year,
            month = month,
            day = day,
        )
        logDebug("showDataPicker OnDateSetListener selected: ${selected.getString()}", TAG)
        dateSelected(selected)
    }
    val datePickerDialog = DatePickerDialog(
        requireContext(),
        onDateSetListener,
        datePickerConfig.selectedValue.getYear(),
        datePickerConfig.selectedValue.getMonth(),
        datePickerConfig.selectedValue.getDay()
    ).apply {
        datePicker.apply {
            minDate = datePickerConfig.minDate.timeInMillis
            maxDate = datePickerConfig.maxDate.timeInMillis
        }
    }
    datePickerDialog.setOnCancelListener {
        dateSelected(null)
    }
    datePickerDialog.show()
}

@Suppress("UNCHECKED_CAST")
fun <T : Fragment> Fragment.findParentFragmentByType(clazz: Class<T>): T? {
    var parent = parentFragment
    while (parent != null && !clazz.isInstance(parent)) {
        parent = parent.parentFragment
    }
    return parent as? T
}