package com.digitall.eid.data.network.administrators

import com.digitall.eid.data.models.network.administrators.AdministratorFrontOfficeResponse
import com.digitall.eid.data.models.network.administrators.AdministratorResponse
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Path

interface AdministratorsApi {

    @GET("raeicei/external/api/v1/eidadministrator/getAll")
    suspend fun getAdministrators(): Response<List<AdministratorResponse>>

    @GET("raeicei/external/api/v1/eidmanagerfrontoffice/getAll/{eidAdministratorId}")
    suspend fun getAdministratorFromOffices(
        @Path("eidAdministratorId") id: String
    ): Response<List<AdministratorFrontOfficeResponse>>
}