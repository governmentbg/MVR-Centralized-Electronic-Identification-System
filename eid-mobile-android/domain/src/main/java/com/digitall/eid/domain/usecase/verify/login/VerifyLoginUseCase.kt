package com.digitall.eid.domain.usecase.verify.login

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.verify.login.VerifyLoginFullModel
import com.digitall.eid.domain.models.verify.login.request.VerifyLoginRequestModel
import com.digitall.eid.domain.models.verify.login.response.VerifyLoginResponseModel
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.repository.network.authentication.AuthenticationNetworkRepository
import com.digitall.eid.domain.repository.network.requests.RequestsNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

class VerifyLoginUseCase : BaseUseCase {

    companion object {
        private const val TAG = "VerifyLoginUseCaseTag"
    }

    private val preferences: PreferencesRepository by inject()
    private val authenticationNetworkRepository: AuthenticationNetworkRepository by inject()
    private val requestsNetworkRepository: RequestsNetworkRepository by inject()

    @Volatile
    private var verifyLoginFullModel = VerifyLoginFullModel()

    fun invoke() = flow {
        val mobileApplicationInstanceId =
            preferences.readApplicationInfo()?.mobileApplicationInstanceId
        val firebaseId = preferences.readFirebaseToken()?.token
        combine(
            authenticationNetworkRepository.verifyLogin(
                data = VerifyLoginRequestModel(
                    mobileApplicationInstanceId = mobileApplicationInstanceId,
                    firebaseId = firebaseId
                )
            ),
            requestsNetworkRepository.getAll(),
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
                            is VerifyLoginResponseModel -> verifyLoginFullModel =
                                verifyLoginFullModel.copy(information = model.model)

                            is List<*> -> verifyLoginFullModel =
                                verifyLoginFullModel.copy(tabToOpen = if (model.model.isNotEmpty()) 2 else 0)
                        }

                        if (verifyLoginFullModel.isValid) {
                            emit(
                                ResultEmittedData.success(
                                    model = verifyLoginFullModel,
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