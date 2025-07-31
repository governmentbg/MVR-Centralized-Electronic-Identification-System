/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.applications.create

import com.digitall.eid.domain.CERTIFICATE_AUTHORITY
import com.digitall.eid.domain.extensions.toBase64
import com.digitall.eid.domain.models.applications.create.ApplicationEnrollWithEIDRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationEnrollWithEIDResponseModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.csr.algorithm.CsrAlgorithm
import com.digitall.eid.domain.models.csr.principal.CsrPrincipalModel
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.repository.network.applications.ApplicationCreateNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.CryptographyHelper
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.FlowCollector
import kotlinx.coroutines.flow.collect
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class ApplicationCreateEnrollWithEIDUseCase : BaseUseCase {

    companion object {
        private const val TAG = "ApplicationCreateConfirmWithEIDUseCaseTag"
    }

    private val cryptographyHelper: CryptographyHelper by inject()
    private val preferences: PreferencesRepository by inject()
    private val applicationCreateNetworkRepository: ApplicationCreateNetworkRepository by inject()

    fun invoke(
        applicationId: String,
        keyAlias: String,
    ): Flow<ResultEmittedData<ApplicationEnrollWithEIDResponseModel>> = flow {
        logDebug("invoke", TAG)
        val userModel = preferences.readApplicationInfo()?.userModel
        val principal = CsrPrincipalModel(
            name = userModel?.nameCyrillic ?: "",
            givenName = userModel?.givenName ?: "",
            surname = userModel?.familyName ?: "",
            country = "BG",
            serialNumber = "PI:BG-${userModel?.eidEntityId}"
        )
        val (_, csrRequest) = cryptographyHelper.generateCSR(
            keyAlias = keyAlias,
            algorithm = CsrAlgorithm.RSA_3072,
            principal = principal,
        )
        logDebug("certificateSigningRequest: $csrRequest", TAG)

        enrollWithEID(
            flow = this@flow,
            applicationId = applicationId,
            certificateSigningRequest = (csrRequest ?: return@flow).toBase64(),
        )
    }.flowOn(Dispatchers.IO)

    private suspend fun enrollWithEID(
        applicationId: String,
        certificateSigningRequest: String,
        flow: FlowCollector<ResultEmittedData<ApplicationEnrollWithEIDResponseModel>>,
    ) {
        logDebug("enrollWithEID", TAG)
        applicationCreateNetworkRepository.enrollWithEID(
            data = ApplicationEnrollWithEIDRequestModel(
                applicationId = applicationId,
                certificateAuthorityName = CERTIFICATE_AUTHORITY,
                certificateSigningRequest = certificateSigningRequest,
            ),
        ).onEach { result ->
            result.onLoading {
                logDebug("enrollWithEID onLoading", TAG)
                flow.emit(ResultEmittedData.loading(model = null))
            }.onSuccess { model, message, responseCode ->
                logDebug("enrollWithEID onSuccess model: $model", TAG)
                if (model.certificate.isNullOrEmpty() ||
                    model.certificateChain.isNullOrEmpty()
                ) {
                    flow.emit(
                        ResultEmittedData.error(
                            model = null,
                            error = null,
                            title = "Server error",
                            responseCode = responseCode,
                            message = "Certificate is empty",
                            errorType = ErrorType.SERVER_DATA_ERROR,
                        )
                    )
                    return@onEach
                }
                flow.emit(
                    ResultEmittedData.success(
                        model = model,
                        message = message,
                        responseCode = responseCode,
                    )
                )
            }.onFailure { error, title, message, responseCode, errorType ->
                logError("enrollWithEID onFailure", message, TAG)
                flow.emit(
                    ResultEmittedData.error(
                        model = null,
                        error = error,
                        title = title,
                        message = message,
                        errorType = errorType,
                        responseCode = responseCode,
                    )
                )
            }
        }.collect()
    }


}