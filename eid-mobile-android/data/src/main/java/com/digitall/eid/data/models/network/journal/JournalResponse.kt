/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.journal

import com.google.gson.annotations.SerializedName

data class JournalResponse(
    @SerializedName("searchAfter") val searchAfter: List<String>?,
    @SerializedName("data") val data: List<JournalResponseItem>?,
)

data class JournalResponseItem(
    @SerializedName("eventId") val eventId: String?,
    @SerializedName("systemId") val systemId: String?,
    @SerializedName("eventDate") val eventDate: String?,
    @SerializedName("checksum") val checksum: String?,
    @SerializedName("sourceFile") val sourceFile: String?,
    @SerializedName("eventType") val eventType: String?,
    @SerializedName("correlationId") val correlationId: String?,
    @SerializedName("message") val message: String?,
    @SerializedName("requesterUserId") val requesterUserId: String?,
    @SerializedName("requesterSystemId") val requesterSystemId: String?,
    @SerializedName("requesterSystemName") val requesterSystemName: String?,
    @SerializedName("targetUserId") val targetUserId: String?,
    @SerializedName("moduleId") val moduleId: String?,
)