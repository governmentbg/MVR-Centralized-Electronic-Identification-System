/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.journal

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.journal.all.JournalModel
import com.digitall.eid.domain.models.journal.all.JournalRequestModel
import kotlinx.coroutines.flow.Flow

interface JournalNetworkRepository {

    fun getJournalFromMe(
        data: JournalRequestModel,
    ): Flow<ResultEmittedData<JournalModel>>

    fun getJournalToMe(
        data: JournalRequestModel,
    ): Flow<ResultEmittedData<JournalModel>>

}