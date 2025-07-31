package com.digitall.eid.domain.usecase.applications.create

import com.digitall.eid.domain.extensions.isOfType
import com.digitall.eid.domain.models.administrators.AdministratorModel
import com.digitall.eid.domain.models.applications.create.ApplicationCreateInitialInformationModel
import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.repository.network.administrators.AdministratorsNetworkRepository
import com.digitall.eid.domain.repository.network.applications.ApplicationCreateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class ApplicationCreateGetInitialInformationUseCase : BaseUseCase {

    companion object {
        private const val TAG = "ApplicationCreateGetInitialInformationUseCaseTag"
    }

    private val applicationCreateNetworkRepository: ApplicationCreateNetworkRepository by inject()
    private val administratorsNetworkRepository: AdministratorsNetworkRepository by inject()

    @Volatile
    private var applicationCreateInitialInformationModel =
        ApplicationCreateInitialInformationModel()

    fun invoke() = flow {
        combine(
            applicationCreateNetworkRepository.getUserDetails(),
            administratorsNetworkRepository.getAdministrators(),
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
                            is ApplicationUserDetailsModel -> applicationCreateInitialInformationModel =
                                applicationCreateInitialInformationModel.copy(userModel = actualModel)

                            is List<*> -> {
                                when {

                                    actualModel.isOfType<AdministratorModel>() -> applicationCreateInitialInformationModel =
                                        applicationCreateInitialInformationModel.copy(administrators = actualModel.filterIsInstance<AdministratorModel>())

                                    else -> {}
                                }
                            }
                        }

                        if (applicationCreateInitialInformationModel.isValid) {
                            emit(
                                ResultEmittedData.success(
                                    model = applicationCreateInitialInformationModel,
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