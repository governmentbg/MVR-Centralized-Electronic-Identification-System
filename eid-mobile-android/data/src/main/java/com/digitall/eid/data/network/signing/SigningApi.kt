/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.network.signing

import com.digitall.eid.data.models.network.signing.SigningCheckUserStatusRequest
import com.digitall.eid.data.models.network.signing.borica.SigningBoricaDownloadResponse
import com.digitall.eid.data.models.network.signing.borica.SigningBoricaSignRequest
import com.digitall.eid.data.models.network.signing.borica.SigningBoricaSignResponse
import com.digitall.eid.data.models.network.signing.borica.SigningBoricaStatusResponse
import com.digitall.eid.data.models.network.signing.borica.SigningBoricaUserStatusResponse
import com.digitall.eid.data.models.network.signing.evrotrust.SigningEvrotrustDownloadResponse
import com.digitall.eid.data.models.network.signing.evrotrust.SigningEvrotrustSignRequest
import com.digitall.eid.data.models.network.signing.evrotrust.SigningEvrotrustSignResponse
import com.digitall.eid.data.models.network.signing.evrotrust.SigningEvrotrustStatusResponse
import com.digitall.eid.data.models.network.signing.evrotrust.SigningEvrotrustUserStatusResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Query

interface SigningApi {

    @GET("signing/api/v1/borica/status")
    suspend fun getBoricaStatus(
        @Query("transactionId") transactionId: String?,
    ): Response<SigningBoricaStatusResponse>

    @GET("signing/api/v1/borica/download")
    suspend fun getBoricaDownload(
        @Query("transactionId") transactionId: String?,
    ): Response<SigningBoricaDownloadResponse>

    @POST("signing/api/v1/borica/sign")
    suspend fun signWithBorica(
        @Body request: SigningBoricaSignRequest,
    ): Response<SigningBoricaSignResponse>

    @POST("signing/api/v1/borica/user/check")
    suspend fun checkBoricaUserStatus(
        @Body request: SigningCheckUserStatusRequest
    ): Response<SigningBoricaUserStatusResponse>

    @GET("signing/api/v1/evrotrust/status")
    suspend fun getEvrotrustStatus(
        @Query("transactionId") transactionId: String?,
    ): Response<SigningEvrotrustStatusResponse>

    @GET("signing/api/v1/evrotrust/download")
    suspend fun getEvrotrustDownload(
        @Query("transactionId") transactionId: String?,
    ): Response<List<SigningEvrotrustDownloadResponse>>

    @POST("signing/api/v1/evrotrust/sign")
    suspend fun signWithEvrotrust(
        @Body request: SigningEvrotrustSignRequest,
    ): Response<SigningEvrotrustSignResponse>

    @POST("signing/api/v1/evrotrust/user/check")
    suspend fun checkEvrotrustUserStatus(
        @Body request: SigningCheckUserStatusRequest
    ): Response<SigningEvrotrustUserStatusResponse>

}