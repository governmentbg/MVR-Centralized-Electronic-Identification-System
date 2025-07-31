/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.network.empowerment

import com.digitall.eid.data.models.network.empowerment.create.create.EmpowermentCreateRequest
import com.digitall.eid.data.models.network.empowerment.create.providers.EmpowermentProvidersResponse
import com.digitall.eid.data.models.network.empowerment.create.services.EmpowermentServiceScopeResponse
import com.digitall.eid.data.models.network.empowerment.create.services.EmpowermentServicesGetResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path
import retrofit2.http.Query

interface EmpowermentCreateApi {

    @GET("ro/api/v1/providers")
    suspend fun getProviders(
        @Query("PageSize") pageSize: Int?,
        @Query("PageIndex") pageIndex: Int?,
        @Query("Name") name: String?,
        @Query("IncludeDeleted") includeDeleted: Boolean?,
        @Query("Status") status: String?,
    ): Response<EmpowermentProvidersResponse>

    @GET("ro/api/v1/providers/services")
    suspend fun getServices(
        @Query("pageSize") pageSize: Int?,
        @Query("pageIndex") pageIndex: Int?,
        @Query("includeEmpowermentOnly") includeEmpowermentOnly: Boolean?,
        @Query("providerid") providerId: String?,
    ): Response<EmpowermentServicesGetResponse>

    @GET("ro/api/v1/providers/services/{serviceId}/scope")
    suspend fun getEmpowermentServiceScope(
        @Path("serviceId") serviceId: String,
        @Query("IncludeDeleted") includeDeleted: Boolean?,
    ): Response<List<EmpowermentServiceScopeResponse>>

    @POST("ro/api/v1/empowerments")
    suspend fun createEmpowerment(
        @Body request: EmpowermentCreateRequest,
    ): Response<List<String>>

}