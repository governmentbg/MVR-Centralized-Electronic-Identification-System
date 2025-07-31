/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 **/
package com.digitall.eid.domain.usecase.empowerment.from.me

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentModel
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentRequestModel
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingByEnum
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingModel
import com.digitall.eid.domain.models.empowerment.common.filter.EmpowermentFilterModel
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class GetEmpowermentFromMeUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetEmpowermentFromMeUseCaseTag"
    }

    private val empowermentNetworkRepository: EmpowermentNetworkRepository by inject()

    fun invoke(
        pageSize: Int,
        pageIndex: Int,
        sortingModel: EmpowermentSortingModel,
        filterModel: EmpowermentFilterModel,
    ): Flow<ResultEmittedData<EmpowermentModel>> {
        var sortBy: String? = sortingModel.sortBy.type
        var sortDirection: String? = sortingModel.sortDirection.type
        if (sortingModel.sortBy == EmpowermentSortingByEnum.DEFAULT) {
            sortBy = null
            sortDirection = null
        }
        return empowermentNetworkRepository.getEmpowermentFromMe(
            data = EmpowermentRequestModel(
                pageSize = pageSize,
                pageIndex = pageIndex,
                number = filterModel.empowermentNumber,
                status = filterModel.status,
                sortBy = sortBy,
                authorizer = filterModel.authorizer,
                onBehalfOf = filterModel.onBehalfOf,
                eik = filterModel.eik,
                serviceName = filterModel.serviceName,
                validToDate = filterModel.validToDate,
                empoweredUids = filterModel.empoweredIDs,
                providerName = filterModel.providerName,
                sortDirection = sortDirection,
                showOnlyNoExpiryDate = filterModel.showOnlyNoExpiryDate,
            )
        )
    }
}