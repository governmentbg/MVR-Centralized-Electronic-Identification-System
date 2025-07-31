/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.applications.all

import com.digitall.eid.domain.models.applications.all.ApplicationsModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.repository.network.applications.ApplicationsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class GetApplicationsUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetApplicationsUseCaseTag"
    }

    private val applicationsNetworkRepository: ApplicationsNetworkRepository by inject()

    fun invoke(
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
    ): Flow<ResultEmittedData<ApplicationsModel>> {
        return applicationsNetworkRepository.getApplications(
            id = id,
            page = page,
            sort = sort,
            size = size,
            status = status,
            createDate = createDate,
            deviceIds = deviceIds,
            applicationType = applicationType,
            eidAdministratorId = eidAdministratorId,
            applicationNumber = applicationNumber,
        )
    }

}