/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.authentication

import com.digitall.eid.domain.models.authentication.request.AuthenticationCertificateRequestModel
import com.digitall.eid.domain.models.authentication.request.AuthenticationChallengeRequestModel
import com.digitall.eid.domain.models.authentication.response.AuthenticationResponseModel
import com.digitall.eid.domain.models.authentication.request.BasicProfileAuthenticationRequestModel
import com.digitall.eid.domain.models.authentication.response.AuthenticationChallengeResponseModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.verify.login.request.VerifyLoginRequestModel
import com.digitall.eid.domain.models.verify.login.response.VerifyLoginResponseModel
import kotlinx.coroutines.flow.Flow

interface AuthenticationNetworkRepository {

    fun authenticateWithBasicProfile(
        data: BasicProfileAuthenticationRequestModel
    ): Flow<ResultEmittedData<AuthenticationResponseModel>>

    fun generateAuthenticationChallenge(
        data: AuthenticationChallengeRequestModel
    ): Flow<ResultEmittedData<AuthenticationChallengeResponseModel>>

    fun authenticateWithCertificate(
        data: AuthenticationCertificateRequestModel
    ): Flow<ResultEmittedData<AuthenticationResponseModel>>

    fun verifyLogin(
        data: VerifyLoginRequestModel
    ): Flow<ResultEmittedData<VerifyLoginResponseModel>>

}