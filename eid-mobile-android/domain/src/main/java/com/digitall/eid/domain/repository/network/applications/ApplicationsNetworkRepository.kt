/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.applications

import com.digitall.eid.domain.models.applications.all.ApplicationCompletionStatusEnum
import com.digitall.eid.domain.models.applications.all.ApplicationDetailsModel
import com.digitall.eid.domain.models.applications.all.ApplicationsModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import kotlinx.coroutines.flow.Flow

interface ApplicationsNetworkRepository {

    fun getApplications(
        page: Int,
        size: Int,
        id: String?,
        sort: String?,
        applicationNumber: String?,
        status: String?,
        createDate: String?,
        deviceIds: List<String>?,
        eidAdministratorId: String?,
        applicationType: List<String>?,
    ): Flow<ResultEmittedData<ApplicationsModel>>

    fun getApplicationDetails(
        id: String,
    ): Flow<ResultEmittedData<ApplicationDetailsModel>>

    fun completeApplication(
        id: String,
    ): Flow<ResultEmittedData<ApplicationCompletionStatusEnum>>

}