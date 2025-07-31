package com.digitall.eid.domain.usecase.empowerment.legal

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentModel
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingByEnum
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingModel
import com.digitall.eid.domain.models.empowerment.common.filter.EmpowermentFilterModel
import com.digitall.eid.domain.models.empowerment.legal.EmpowermentLegalRequestModel
import com.digitall.eid.domain.repository.network.empowerment.EmpowermentNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class GetEmpowermentLegalUseCase: BaseUseCase {

    companion object {
        private const val TAG = "GetEmpowermentLegalUseCaseTag"
    }

    private val empowermentNetworkRepository: EmpowermentNetworkRepository by inject()

    fun invoke(
        pageSize: Int,
        pageIndex: Int,
        legalNumber: String?,
        sortingModel: EmpowermentSortingModel,
        filterModel: EmpowermentFilterModel,
    ): Flow<ResultEmittedData<EmpowermentModel>> {
        var sortBy: String? = sortingModel.sortBy.type
        var sortDirection: String? = sortingModel.sortDirection.type
        if (sortingModel.sortBy == EmpowermentSortingByEnum.DEFAULT) {
            sortBy = null
            sortDirection = null
        }
        return empowermentNetworkRepository.getEmpowermentLegal(
            data = EmpowermentLegalRequestModel(
                pageSize = pageSize,
                pageIndex = pageIndex,
                legalNumber = legalNumber,
                status = filterModel.status,
                sortBy = sortBy,
                serviceName = filterModel.serviceName,
                validToDate = filterModel.validToDate,
                authorizerUids = filterModel.empoweredIDs,
                providerName = filterModel.providerName,
                sortDirection = sortDirection,
                showOnlyNoExpiryDate = filterModel.showOnlyNoExpiryDate,
            )
        )
    }
}