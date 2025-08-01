package com.digitall.eid.data.network.payments

import com.digitall.eid.data.models.network.payments.history.response.PaymentHistoryResponse
import retrofit2.Response
import retrofit2.http.GET

interface PaymentsApi {

    @GET("mpozei/external/api/v1/payments")
    suspend fun getHistory(): Response<List<PaymentHistoryResponse>>
}