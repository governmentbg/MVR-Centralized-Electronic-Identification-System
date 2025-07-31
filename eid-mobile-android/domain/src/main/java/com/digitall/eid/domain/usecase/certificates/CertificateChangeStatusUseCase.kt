package com.digitall.eid.domain.usecase.certificates

import com.digitall.eid.domain.models.certificates.CertificateStatusChangeRequestModel
import com.digitall.eid.domain.repository.network.certificates.CertificatesNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import org.koin.core.component.inject

class CertificateChangeStatusUseCase : BaseUseCase {

    companion object {
        private const val TAG = "CertificateStopUseCaseTag"
    }

    private val certificatesNetworkRepository: CertificatesNetworkRepository by inject()

    fun invoke(changeRequestModel: CertificateStatusChangeRequestModel) =
        certificatesNetworkRepository.certificateChangeStatus(changeRequestModel = changeRequestModel)
}