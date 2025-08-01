/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.network.applications

import com.digitall.eid.data.models.network.applications.all.ApplicationDetailsResponse
import com.digitall.eid.data.models.network.applications.all.ApplicationsResponse
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path
import retrofit2.http.Query

interface ApplicationsApi {

    @GET("mpozei/external/api/v1/applications/find")
    suspend fun getApplications(
        @Query("page") page: Int,
        @Query("size") size: Int,
        @Query("id") id: String?,
        @Query("applicationNumber") applicationNumber: String?,
        @Query("sort") sort: String?,
        @Query("status") status: String?,
        @Query("createDate") createDate: String?,
        @Query("deviceId") deviceId: List<String>?,
        @Query("eidAdministratorId") eidAdministratorId: String?,
        @Query("applicationType") applicationType: List<String>?,
    ): Response<ApplicationsResponse>

    @GET("mpozei/external/api/v1/applications/{id}")
    suspend fun getApplicationDetails(
        @Path("id") id: String,
    ): Response<ApplicationDetailsResponse>

    @POST("mpozei/external/api/v1/applications/{id}/complete")
    suspend fun completeApplication(
        @Path("id") id: String
    ): Response<String>

}