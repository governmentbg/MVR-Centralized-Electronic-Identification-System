/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.journal

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.journal.all.JournalModel
import com.digitall.eid.domain.models.journal.all.JournalRequestModel
import com.digitall.eid.domain.repository.network.journal.JournalNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class GetJournalToMeUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetJournalToMeUseCaseTag"
    }

    private val journalNetworkRepository: JournalNetworkRepository by inject()

    fun invoke(
        data: JournalRequestModel,
    ): Flow<ResultEmittedData<JournalModel>> {
        return journalNetworkRepository.getJournalToMe(
            data = data
        )
    }

}