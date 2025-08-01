package com.digitall.eid.domain.models.journal.all

data class JournalRequestModel(
    val startDate: String?,
    val endDate: String?,
    val eventTypes: List<String>?,
    val cursorSize: Int?,
    val cursorSearchAfter: List<String>?
)
