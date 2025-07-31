/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.network.empowerment

import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentRequest
import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentResponse
import com.digitall.eid.data.models.network.empowerment.legal.EmpowermentLegalRequest
import com.digitall.eid.data.models.network.empowerment.signing.EmpowermentSigningSignRequest
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path
import retrofit2.http.Query

interface EmpowermentApi {

    @POST("ro/api/v1/empowerments/from")
    suspend fun getEmpowermentFromMe(
        @Body request: EmpowermentRequest,
    ): Response<EmpowermentResponse>

    @GET("ro/api/v1/empowerments/to")
    suspend fun getEmpowermentToMe(
        @Query("PageSize") pageSize: Int?,
        @Query("SortBy") sortBy: String?,
        @Query("Number") number: String?,
        @Query("Status") status: String?,
        @Query("PageIndex") pageIndex: Int?,
        @Query("Authorizer") authorizer: String?,
        @Query("ServiceName") serviceName: String?,
        @Query("ValidToDate") validToDate: String?,
        @Query("ProviderName") providerName: String?,
        @Query("SortDirection") sortDirection: String?,
        @Query("OnBehalfOf") onBehalfOf: String?,
        @Query("Eik") eik: String?,
        @Query("ShowOnlyNoExpiryDate") showOnlyNoExpiryDate: Boolean?,
    ): Response<EmpowermentResponse>

    @POST("ro/api/v1/empowerments/eik")
    suspend fun getEmpowermentLegal(
        @Body request: EmpowermentLegalRequest,
    ): Response<EmpowermentResponse>

    @POST("ro/api/v1/empowerments/{empowermentId}/sign")
    suspend fun signEmpowerment(
        @Path("empowermentId") empowermentId: String,
        @Body request: EmpowermentSigningSignRequest,
    ): Response<String>



}