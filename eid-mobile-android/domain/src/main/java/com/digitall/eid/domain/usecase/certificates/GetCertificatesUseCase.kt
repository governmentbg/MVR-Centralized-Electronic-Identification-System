/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.certificates

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.certificates.CertificatesModel
import com.digitall.eid.domain.repository.network.certificates.CertificatesNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class GetCertificatesUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetCertificatesUseCaseTag"
    }

    private val certificatesNetworkRepository: CertificatesNetworkRepository by inject()

    fun invoke(
        page: Int,
        size: Int,
        id: String?,
        sort: String?,
        status: String?,
        alias: String?,
        serialNumber: String?,
        validityFrom: String?,
        validityUntil: String?,
        deviceType: List<String>?,
        eidAdministratorId: String?,
    ): Flow<ResultEmittedData<CertificatesModel>> {
        return certificatesNetworkRepository.getCertificates(
            id = id,
            page = page,
            sort = sort,
            size = size,
            alias = alias,
            status = status,
            deviceType = deviceType,
            validityFrom = validityFrom,
            serialNumber = serialNumber,
            validityUntil = validityUntil,
            eidAdministratorId = eidAdministratorId,
        )
    }

}