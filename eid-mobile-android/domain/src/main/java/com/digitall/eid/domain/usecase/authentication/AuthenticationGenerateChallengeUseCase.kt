package com.digitall.eid.domain.usecase.authentication

import com.digitall.eid.domain.models.authentication.request.AuthenticationChallengeRequestModel
import com.digitall.eid.domain.models.authentication.response.AuthenticationChallengeResponseModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.common.LevelOfAssurance
import com.digitall.eid.domain.repository.network.authentication.AuthenticationNetworkRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import kotlinx.coroutines.flow.Flow
import org.koin.core.component.inject

class AuthenticationGenerateChallengeUseCase : BaseUseCase {

    companion object {
        private const val TAG = "AuthenticationGenerateChallengeUseCaseTag"
    }

    private val authenticationNetworkRepository: AuthenticationNetworkRepository by inject()

    fun invoke(
        levelOfAssurance: LevelOfAssurance
    ): Flow<ResultEmittedData<AuthenticationChallengeResponseModel>> {
        logDebug("authenticationGenerateChallenge levelOfAssurance: ${levelOfAssurance.type}", TAG)
        return authenticationNetworkRepository.generateAuthenticationChallenge(
            data = AuthenticationChallengeRequestModel(
                requestForm = null,
                levelOfAssurance = levelOfAssurance
            )
        )
    }
}