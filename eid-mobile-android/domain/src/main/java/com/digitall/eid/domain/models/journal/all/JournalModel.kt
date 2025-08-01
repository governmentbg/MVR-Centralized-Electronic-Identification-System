/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.journal.all

data class JournalModel(
    val searchAfter: List<String>?,
    val data: List<JournalModelItem>?,
)

data class JournalModelItem(
    val eventId: String?,
    val systemId: String?,
    val eventDate: String?,
    val checksum: String?,
    val sourceFile: String?,
    val eventType: String?,
    val correlationId: String?,
    val message: String?,
    val requesterUserId: String?,
    val requesterSystemId: String?,
    val requesterSystemName: String?,
    val targetUserId: String?,
    val moduleId: String?,
)