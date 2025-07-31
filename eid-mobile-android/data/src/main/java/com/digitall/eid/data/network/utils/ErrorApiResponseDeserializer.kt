package com.digitall.eid.data.network.utils

import com.digitall.eid.data.models.network.base.ErrorApiResponse
import com.google.gson.JsonDeserializationContext
import com.google.gson.JsonDeserializer
import com.google.gson.JsonElement
import com.google.gson.reflect.TypeToken
import java.lang.reflect.Type

class ErrorApiResponseDeserializer: JsonDeserializer<ErrorApiResponse> {

    private val dictionaryType = object: TypeToken<Map<String, Any>>() {}.type
    private val arrayType = object: TypeToken<List<String>>() {}.type

    override fun deserialize(
        json: JsonElement,
        typeOfT: Type,
        context: JsonDeserializationContext
    ): ErrorApiResponse {
        val errorJson = json.asJsonObject
        val status = errorJson.asJsonObject["status"]
        val title = errorJson.asJsonObject["title"]
        val detail = errorJson.asJsonObject["detail"]
        val errors = errorJson.asJsonObject["errors"]

        return ErrorApiResponse(
            status = if (status?.isJsonNull == true) null else status.asInt,
            title = if (title?.isJsonNull == true) null else title.asString,
            detail = detail?.asString,
            errors = if (errors?.isJsonNull == null) null else when {
                errors.isJsonObject -> context.deserialize(errors, dictionaryType) as Map<String, Any>
                errors.isJsonArray -> context.deserialize(errors, arrayType) as List<String>
                else -> null
            }
        )
    }
}