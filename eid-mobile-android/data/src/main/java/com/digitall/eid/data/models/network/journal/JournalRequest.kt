/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.journal

import com.google.gson.annotations.SerializedName

data class JournalRequest(
    @SerializedName("startDate") val startDate: String?,
    @SerializedName("endDate") val endDate: String?,
    @SerializedName("eventTypes") val eventTypes: List<String>?,
    @SerializedName("cursorSize") val cursorSize: Int?,
    @SerializedName("cursorSearchAfter") val cursorSearchAfter: List<String?>?,
)