/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.network.certificates

import com.digitall.eid.data.models.network.base.EmptyResponse
import com.digitall.eid.data.models.network.certificates.request.CertificateAliasChangeRequest
import com.digitall.eid.data.models.network.certificates.response.CertificateDetailsResponse
import com.digitall.eid.data.models.network.certificates.request.CertificateStatusChangeRequest
import com.digitall.eid.data.models.network.certificates.response.CertificateHistoryElementResponse
import com.digitall.eid.data.models.network.certificates.response.CertificateStatusChangeResponse
import com.digitall.eid.data.models.network.certificates.response.CertificatesResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path
import retrofit2.http.Query

interface CertificatesApi {

    @GET("mpozei/external/api/v1/certificates/find")
    suspend fun getCertificates(
        @Query("id") id: String?,
        @Query("page") page: Int,
        @Query("size") size: Int,
        @Query("sort") sort: String?,
        @Query("alias") alias: String?,
        @Query("status") status: String?,
        @Query("serialNumber") serialNumber: String?,
        @Query("validityFrom") validityFrom: String?,
        @Query("validityUntil") validityUntil: String?,
        @Query("deviceId") deviceId: List<String>?,
        @Query("eidAdministratorId") eidAdministratorId: String?,
    ): Response<CertificatesResponse>

    @GET("mpozei/external/api/v1/certificates/{id}")
    suspend fun getCertificateDetails(
        @Path("id") id: String,
    ): Response<CertificateDetailsResponse>

    @GET("mpozei/external/api/v1/certificates/{id}/history")
    suspend fun getCertificateHistory(
        @Path("id") id: String,
    ): Response<List<CertificateHistoryElementResponse>>

    @POST("mpozei/external/api/v1/applications/certificate-status-change/plain")
    suspend fun changeCertificateStatus(
        @Body requestBody: CertificateStatusChangeRequest
    ): Response<CertificateStatusChangeResponse>

    @PUT("mpozei/external/api/v1/certificates/alias")
    suspend fun setCertificateAlias(
        @Query("certificateId") id: String,
        @Body requestBody: CertificateAliasChangeRequest,
    ): Response<EmptyResponse>

}