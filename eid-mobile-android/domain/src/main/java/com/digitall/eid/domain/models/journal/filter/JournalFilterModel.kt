package com.digitall.eid.domain.models.journal.filter

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class JournalFilterModel(
    val startDate: String?,
    val endDate: String?,
    val eventTypes: List<String>,
    val allEventTypesSelected: Boolean = false,
) : Parcelable {

    val allPropertiesAreNull: Boolean
        get() {
            val primitiveMemberProps = listOf(
                startDate,
                endDate,
            )
            val listMemberProps = listOf(
                eventTypes,
            )
            val arePrimitivesNotInit = primitiveMemberProps.all { member -> member == null }
            val areListNotInit = listMemberProps.all { list -> list.isEmpty() }
            return arePrimitivesNotInit && areListNotInit
        }
}
