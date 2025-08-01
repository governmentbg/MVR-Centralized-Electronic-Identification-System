/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.signing

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.signing.SigningCheckUserStatusRequestModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaDownloadResponseModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustDownloadResponseModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaSignRequestModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaSignResponseModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustSignRequestModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustSignResponseModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaStatusResponseModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustStatusResponseModel
import com.digitall.eid.domain.models.signing.borica.SigningBoricaUserStatusResponseModel
import com.digitall.eid.domain.models.signing.evrotrust.SigningEvrotrustUserStatusResponseModel
import kotlinx.coroutines.flow.Flow

interface SigningNetworkRepository {

    fun checkBoricaUserStatus(
        data: SigningCheckUserStatusRequestModel
    ): Flow<ResultEmittedData<SigningBoricaUserStatusResponseModel>>

    fun signWithBorica(
        request: SigningBoricaSignRequestModel,
    ): Flow<ResultEmittedData<SigningBoricaSignResponseModel>>

    fun getBoricaStatus(
        transactionId: String,
    ): Flow<ResultEmittedData<SigningBoricaStatusResponseModel>>

    fun getBoricaDownload(
        transactionId: String,
    ): Flow<ResultEmittedData<SigningBoricaDownloadResponseModel>>

    fun checkEvrotrustUserStatus(
        data: SigningCheckUserStatusRequestModel
    ): Flow<ResultEmittedData<SigningEvrotrustUserStatusResponseModel>>

    fun signWithEvrotrust(
        request: SigningEvrotrustSignRequestModel,
    ): Flow<ResultEmittedData<SigningEvrotrustSignResponseModel>>

    fun getEvrotrustStatus(
        transactionId: String,
    ): Flow<ResultEmittedData<SigningEvrotrustStatusResponseModel>>

    fun getEvrotrustDownload(
        transactionId: String,
    ): Flow<ResultEmittedData<List<SigningEvrotrustDownloadResponseModel>>>

}