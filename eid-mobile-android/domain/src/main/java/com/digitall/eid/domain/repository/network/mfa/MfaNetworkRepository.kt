package com.digitall.eid.domain.repository.network.mfa

import com.digitall.eid.domain.models.authentication.response.AuthenticationResponseModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.mfa.request.GenerateNewOtpCodeRequestModel
import com.digitall.eid.domain.models.mfa.request.VerifyOtpCodeRequestModel
import kotlinx.coroutines.flow.Flow

interface MfaNetworkRepository {

    fun verifyOtpCode(
        data: VerifyOtpCodeRequestModel,
    ): Flow<ResultEmittedData<AuthenticationResponseModel>>

    fun generateNewOtpCode(
        data: GenerateNewOtpCodeRequestModel,
    ): Flow<ResultEmittedData<Unit>>
}