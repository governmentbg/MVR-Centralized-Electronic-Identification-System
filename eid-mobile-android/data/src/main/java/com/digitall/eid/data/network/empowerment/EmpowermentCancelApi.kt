/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.network.empowerment

import com.digitall.eid.data.models.network.base.EmptyResponse
import com.digitall.eid.data.models.network.empowerment.cancel.EmpowermentCancelRequestModel
import com.digitall.eid.data.models.network.empowerment.common.EmpowermentReasonResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

interface EmpowermentCancelApi {

    @GET("ro/api/v1/empowerments/withdraw/reasons")
    suspend fun getEmpowermentFromMeCancelReasons(): Response<List<EmpowermentReasonResponse>>

    @GET("ro/api/v1/empowerments/disagreement/reasons")
    suspend fun getEmpowermentToMeCancelReasons(): Response<List<EmpowermentReasonResponse>>

    @POST("ro/api/v1/empowerments/{empowermentId}/withdraw")
    suspend fun cancelEmpowermentFromMe(
        @Path("empowermentId") empowermentId: String,
        @Body request: EmpowermentCancelRequestModel,
    ): Response<EmptyResponse>

    @POST("ro/api/v1/empowerments/{empowermentId}/disagreement")
    suspend fun cancelEmpowermentToMe(
        @Path("empowermentId") empowermentId: String,
        @Body request: EmpowermentCancelRequestModel,
    ): Response<EmptyResponse>

}