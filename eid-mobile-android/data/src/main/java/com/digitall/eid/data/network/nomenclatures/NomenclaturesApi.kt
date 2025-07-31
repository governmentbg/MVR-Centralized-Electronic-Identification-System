package com.digitall.eid.data.network.nomenclatures

import com.digitall.eid.data.models.network.nomenclatures.reasons.NomenclaturesReasonsResponse
import retrofit2.Response
import retrofit2.http.GET

interface NomenclaturesApi {

    @GET("mpozei/external/api/v1/nomenclatures/reasons")
    suspend fun getReasons(): Response<List<NomenclaturesReasonsResponse>>
}