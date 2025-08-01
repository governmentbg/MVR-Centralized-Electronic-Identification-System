package com.digitall.eid.data.network.assets

import com.digitall.eid.data.models.network.assets.localization.LocalizationsResponse
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Path

interface AssetsApi {

    @GET("assets/i18n/{language}.json")
    suspend fun getLocalizations(
        @Path("language") language: String
    ): Response<LocalizationsResponse>

}