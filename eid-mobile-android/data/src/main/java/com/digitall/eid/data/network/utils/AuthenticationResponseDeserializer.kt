package com.digitall.eid.data.network.utils

import com.digitall.eid.data.models.network.authentication.response.AuthenticationResponse
import com.digitall.eid.data.models.network.authentication.response.MFAResponse
import com.digitall.eid.data.models.network.authentication.response.TokenResponse
import com.google.gson.JsonDeserializationContext
import com.google.gson.JsonDeserializer
import com.google.gson.JsonElement
import com.google.gson.reflect.TypeToken
import java.lang.reflect.Type

class AuthenticationResponseDeserializer : JsonDeserializer<AuthenticationResponse> {

    private val tokenResponse = object : TypeToken<TokenResponse>() {}.type
    private val mfaResponse = object : TypeToken<MFAResponse>() {}.type

    override fun deserialize(
        json: JsonElement,
        typeOfT: Type,
        context: JsonDeserializationContext
    ): AuthenticationResponse {
        val jsonObject = json.asJsonObject

        return AuthenticationResponse(
            data = if (jsonObject.has("sessionId"))
                context.deserialize(json, mfaResponse) as MFAResponse
            else context.deserialize(json, tokenResponse) as TokenResponse
        )
    }
}