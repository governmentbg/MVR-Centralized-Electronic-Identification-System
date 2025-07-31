/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.network.journal

import com.digitall.eid.data.models.network.journal.JournalRequest
import com.digitall.eid.data.models.network.journal.JournalResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface JournalApi {

    @POST("pjs/api/v1/log/from")
    suspend fun getJournalFromMe(
        @Body request: JournalRequest,
    ): Response<JournalResponse>


    @POST("pjs/api/v1/log/to")
    suspend fun getJournalToMe(
        @Body request: JournalRequest,
    ): Response<JournalResponse>

}