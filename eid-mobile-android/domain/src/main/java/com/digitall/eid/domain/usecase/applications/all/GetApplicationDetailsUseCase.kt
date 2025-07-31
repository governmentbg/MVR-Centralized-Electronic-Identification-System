/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.applications.all

import com.digitall.eid.domain.extensions.isOfType
import com.digitall.eid.domain.models.applications.all.ApplicationDetailsModel
import com.digitall.eid.domain.models.applications.all.ApplicationFullDetailsModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.nomenclatures.reasons.NomenclaturesReasonsModel
import com.digitall.eid.domain.repository.network.applications.ApplicationsNetworkRepository
import com.digitall.eid.domain.repository.network.nomenclatures.NomenclaturesNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class GetApplicationDetailsUseCase : BaseUseCase {

    companion object {
        private const val TAG = "GetApplicationDetailsUseCaseTag"
    }

    private val applicationsNetworkRepository: ApplicationsNetworkRepository by inject()
    private val nomenclaturesNetworkRepository: NomenclaturesNetworkRepository by inject()

    @Volatile
    private var applicationFullDetailsModel = ApplicationFullDetailsModel()

    fun invoke(id: String) = flow {
        combine(
            nomenclaturesNetworkRepository.getReasons(),
            applicationsNetworkRepository.getApplicationDetails(id = id)
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
                            is ApplicationDetailsModel -> applicationFullDetailsModel =
                                applicationFullDetailsModel.copy(information = actualModel)

                            is List<*> ->
                                when {

                                    actualModel.isOfType<NomenclaturesReasonsModel>() -> applicationFullDetailsModel =
                                        applicationFullDetailsModel.copy(nomenclatures = actualModel.filterIsInstance<NomenclaturesReasonsModel>())

                                    else -> {}
                                }
                        }

                        if (applicationFullDetailsModel.isValid) {
                            emit(
                                ResultEmittedData.success(
                                    model = applicationFullDetailsModel,
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