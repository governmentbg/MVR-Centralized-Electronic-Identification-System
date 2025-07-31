/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.journal

import com.digitall.eid.data.mappers.base.BaseMapperWithData
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDateToTextDate
import com.digitall.eid.domain.models.assets.localization.logs.LogLocalizationModel
import com.digitall.eid.domain.models.journal.all.JournalModelItem
import com.digitall.eid.models.journal.to.me.JournalToMeUi

class JournalToMeUiMapper :
    BaseMapperWithData<JournalModelItem, List<LogLocalizationModel>, JournalToMeUi>() {

    override fun map(from: JournalModelItem, data: List<LogLocalizationModel>?): JournalToMeUi {
        return with(from) {
            JournalToMeUi(
                eventId = eventId ?: "Unknown",
                eventDate = eventDate?.fromServerDateToTextDate(
                    dateFormat = UiDateFormats.WITH_TIME,
                ) ?: "Unknown",
                eventType = data?.firstOrNull { model -> model.type == eventType }?.description
                    ?: eventType ?: "Unknown",
                system = requesterSystemName ?: "Unknown",
            )
        }
    }

}