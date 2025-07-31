/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.certificates

import com.digitall.eid.domain.extensions.isOfType
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.certificates.CertificateDetailsModel
import com.digitall.eid.domain.models.certificates.CertificateFullDetailsModel
import com.digitall.eid.domain.models.certificates.CertificateHistoryElementModel
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclaturesReasonsModel
import com.digitall.eid.domain.repository.network.certificates.CertificatesNetworkRepository
import com.digitall.eid.domain.repository.network.nomenclatures.NomenclaturesNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject
import kotlin.concurrent.Volatile

class GetCertificateDetailsUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetCertificateDetailsUseCaseTag"
    }

    private val nomenclaturesNetworkRepository: NomenclaturesNetworkRepository by inject()
    private val certificatesNetworkRepository: CertificatesNetworkRepository by inject()

    @Volatile
    private var certificateFullDetails = CertificateFullDetailsModel()

    fun invoke(id: String) = flow {
        combine(
            nomenclaturesNetworkRepository.getReasons(),
            certificatesNetworkRepository.getCertificateDetails(id = id),
            certificatesNetworkRepository.getCertificateHistory(id = id),
        ) { results ->
            results
        }.collect { results ->
            when {
                results.all { it.status == ResultEmittedData.Status.LOADING } -> emit(
                    ResultEmittedData.loading(model = null)
                )

                results.all { it.status == ResultEmittedData.Status.SUCCESS } -> {
                    results.onEach { model ->
                        when (val actualModel = model.model) {
                            is CertificateDetailsModel -> certificateFullDetails =
                                certificateFullDetails.copy(information = actualModel)

                            is List<*> ->
                                when {
                                    actualModel.isOfType<CertificateHistoryElementModel>() -> certificateFullDetails =
                                        certificateFullDetails.copy(history = actualModel.filterIsInstance<CertificateHistoryElementModel>())

                                    actualModel.isOfType<NomenclaturesReasonsModel>() -> certificateFullDetails =
                                        certificateFullDetails.copy(nomenclatures = actualModel.filterIsInstance<NomenclaturesReasonsModel>())

                                    else -> {}
                                }
                        }

                        if (certificateFullDetails.isValid) {
                            emit(
                                ResultEmittedData.success(
                                    model = certificateFullDetails,
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