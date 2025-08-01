/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.utils

import android.util.Base64
import com.digitall.eid.data.mappers.network.user.UserMapper
import com.digitall.eid.data.models.network.user.UserJsonData
import com.digitall.eid.domain.models.user.UserModel
import com.digitall.eid.domain.utils.JWTDecoder
import com.google.gson.Gson
import org.json.JSONObject
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class JWTDecoderImpl : JWTDecoder, KoinComponent {

    private val userMapper: UserMapper by inject()

    override fun getUser(fromToken: String): UserModel? {
        return try {
            val jsonDict = decodeJWTToken(fromToken)
            val user = Gson().fromJson(JSONObject(jsonDict).toString(), UserJsonData::class.java)
            return userMapper.map(user)
        } catch (e: Exception) {
            null
        }
    }

    private fun base64Decode(base64: String): ByteArray {
        val cleaned = base64
            .replace("-", "+")
            .replace("_", "/")
        val padded = cleaned.padEnd(cleaned.length + (4 - cleaned.length % 4) % 4, '=')
        return Base64.decode(padded, Base64.DEFAULT) ?: throw Exception()
    }

    private fun decodeJWTPart(value: String): Map<String, Any> {
        val bodyData = base64Decode(value)
        val json = JSONObject(String(bodyData))
        return json.toMap() // Convert JSONObject to Map
    }

    private fun decodeJWTToken(jwt: String): Map<String, Any> {
        val segments = jwt.split(".")
        if (segments.size < 2) throw Exception()
        return decodeJWTPart(segments[1])
    }

    private fun JSONObject.toMap(): Map<String, Any> =
        keys().asSequence().associateWith { key ->
            when (val value = this[key]) {
                is JSONObject -> value.toMap()
                else -> value
            }
        }


}