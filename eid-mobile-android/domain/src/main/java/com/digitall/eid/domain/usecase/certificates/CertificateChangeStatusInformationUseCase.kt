package com.digitall.eid.domain.usecase.certificates

import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.certificates.CertificateChangeStatusInformationModel
import com.digitall.eid.domain.models.certificates.CertificateDetailsModel
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclaturesReasonsModel
import com.digitall.eid.domain.repository.network.applications.ApplicationCreateNetworkRepository
import com.digitall.eid.domain.repository.network.certificates.CertificatesNetworkRepository
import com.digitall.eid.domain.repository.network.nomenclatures.NomenclaturesNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject
import kotlin.concurrent.Volatile

class CertificateChangeStatusInformationUseCase : BaseUseCase {

    private val certificatesNetworkRepository: CertificatesNetworkRepository by inject()
    private val nomenclaturesNetworkRepository: NomenclaturesNetworkRepository by inject()
    private val applicationCreateNetworkRepository: ApplicationCreateNetworkRepository by inject()

    @Volatile
    private var certificateChangeStatusInformationModel = CertificateChangeStatusInformationModel()

    fun invoke(
        id: String,
    ) = flow {
        combine(
            certificatesNetworkRepository.getCertificateDetails(id = id),
            nomenclaturesNetworkRepository.getReasons(),
            applicationCreateNetworkRepository.getUserDetails()
        ) { results ->
            results
        }.collect { results ->
                when {
                    results.all { it.status == ResultEmittedData.Status.LOADING } -> emit(
                        ResultEmittedData.loading(model = null)
                    )

                    results.all { it.status == ResultEmittedData.Status.SUCCESS } -> {
                        results.onEach { model ->
                            when (model.model) {
                                is ApplicationUserDetailsModel -> certificateChangeStatusInformationModel =
                                    certificateChangeStatusInformationModel.copy(userDetails = model.model)

                                is CertificateDetailsModel -> certificateChangeStatusInformationModel =
                                    certificateChangeStatusInformationModel.copy(certificateDetails = model.model)

                                is List<*> -> certificateChangeStatusInformationModel =
                                    certificateChangeStatusInformationModel.copy(reasons = model.model.filterIsInstance<NomenclaturesReasonsModel>())
                            }

                            if (certificateChangeStatusInformationModel.isValid) {
                                emit(
                                    ResultEmittedData.success(
                                        model = certificateChangeStatusInformationModel,
                                        message = model.message,
                                        responseCode = model.responseCode
                                    )
                                )
                            }
                        }
                    }

                    results.any { it.status == ResultEmittedData.Status.ERROR } -> {
                        results.firstOrNull { it.status == ResultEmittedData.Status.ERROR }
                            ?.let { model ->
                                emit(
                                    ResultEmittedData.error(
                                        model = null,
                                        error = model.error,
                                        title = model.title,
                                        message = model.message,
                                        errorType = model.errorType,
                                        responseCode = model.responseCode,
                                    )
                                )
                            }
                    }
                }
            }
    }.flowOn(Dispatchers.IO)
}