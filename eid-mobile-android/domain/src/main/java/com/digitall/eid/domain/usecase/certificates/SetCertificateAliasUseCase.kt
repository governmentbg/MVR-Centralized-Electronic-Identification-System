package com.digitall.eid.domain.usecase.certificates

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.certificates.CertificateAliasChangeRequestModel
import com.digitall.eid.domain.repository.network.certificates.CertificatesNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class SetCertificateAliasUseCase : BaseUseCase {

    companion object {
        private const val TAG = "SetCertificateAliasUseCaseTag"
    }

    private val certificatesNetworkRepository: CertificatesNetworkRepository by inject()

    fun invoke(id: String, alias: String): Flow<ResultEmittedData<Unit>> {
        logDebug("setCertificateAlias", TAG)
        return certificatesNetworkRepository.setCertificateAlias(
            id = id,
            aliasRequestModel = CertificateAliasChangeRequestModel(alias = alias)
        )
    }
}